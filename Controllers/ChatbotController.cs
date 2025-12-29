using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rh.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Rh.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        private readonly string _groqApiKey = "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";

        public ChatbotController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> GetResponse(string message)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var role = HttpContext.Session.GetString("UserRole");
                if (userId == null) return Json(new { reply = "Session expirée. Connectez-vous." });

                var aujourdHui = DateTime.Today;
                string dataBriefing = "";

                // --- 1. RÉCUPÉRATION DES DONNÉES (ADMIN vs EMPLOYÉ) ---
                if (role == "Admin")
                {
                    var emps = await _context.Employees
                        .Select(e => $"- {e.Prenom}: {e.SalaireBase:F2}€, Solde {e.JoursCongesRestants}j")
                        .ToListAsync();

                    var abs = await _context.Conges.Include(c => c.Employe)
                        .Where(c => c.Statut == "Approuvé" && aujourdHui >= c.DateDebut && aujourdHui <= c.DateFin)
                        .Select(c => $"- {c.Employe.Prenom} est en {c.TypeConge} (Fin: {c.DateFin:dd/MM})")
                        .ToListAsync();

                    var wait = await _context.Conges.Include(c => c.Employe)
                        .Where(c => c.Statut == "En attente")
                        .Select(c => $"- Demande de {c.Employe.Prenom} ({c.TypeConge}) du {c.DateDebut:dd/MM} au {c.DateFin:dd/MM}")
                        .ToListAsync();

                    dataBriefing = $@"
                    [EFFECTIFS ET SALAIRES]
                    {string.Join("\n", emps)}

                    [ABSENCES AUJOURD'HUI]
                    {(abs.Any() ? string.Join("\n", abs) : "Aucun absent.")}

                    [CONGÉS EN ATTENTE]
                    {(wait.Any() ? string.Join("\n", wait) : "Aucune demande en attente.")}";
                }
                else
                {
                    // MODE EMPLOYÉ : On récupère ses infos + tous ses congés
                    var u = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == userId);

                    var sesConges = await _context.Conges
                        .Where(c => c.EmployeId == userId)
                        .OrderByDescending(c => c.DateDebut)
                        .Select(c => $"- {c.TypeConge} du {c.DateDebut:dd/MM} au {c.DateFin:dd/MM} (Statut: {c.Statut})")
                        .ToListAsync();

                    dataBriefing = $@"
                    [VOTRE PROFIL]
                    Prénom: {u.Prenom}
                    Solde: {u.JoursCongesRestants} jours
                    Salaire: {u.SalaireBase:F2}€

                    [VOS CONGÉS ET DEMANDES]
                    {(sesConges.Any() ? string.Join("\n", sesConges) : "Aucun historique de congé.")}";
                }

                // --- 2. PROMPT SYSTÈME OPTIMISÉ ---
                string systemPrompt = $@"Tu es l'assistant RH. Nous sommes le {aujourdHui:dd/MM/yyyy}.
                Données disponibles :
                {dataBriefing}

                RÈGLES :
                1. RÉPONSE COURTE : Va droit au but (1 phrase max si possible).
                2. SALUTATIONS : Si on dit ""Bonjour"", réponds juste ""Bonjour, comment puis-je vous aider ?"" sans déballer les chiffres.
                3. STATUTS : Pour savoir si un congé est validé ou en cours, regarde le 'Statut' dans la liste.
                4. PRÉCISION : Ne devine rien. Si l'info n'est pas là, dis ""Je n'ai pas cette information"".";

                // --- 3. APPEL API ---
                string aiReply = await CallGroqApi(systemPrompt, message);
                return Json(new { reply = aiReply });
            }
            catch (Exception ex)
            {
                // Log de l'erreur pour le développeur
                Console.WriteLine(ex.Message);
                return Json(new { reply = "Le service est temporairement indisponible." });
            }
        }

        private async Task<string> CallGroqApi(string systemPrompt, string userMessage)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _groqApiKey);

            var body = new
            {
                model = "llama-3.1-8b-instant",
                messages = new[] {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                temperature = 0.1,
                max_tokens = 200
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var res = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

            if (!res.IsSuccessStatusCode) return "Service indisponible (Erreur API).";

            var resString = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resString);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}