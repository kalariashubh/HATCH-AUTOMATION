using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using hatch_automation.Services;

namespace hatch_automation.Commands
{
    public class HatchCommands
    {
        [CommandMethod("AUTOHATCH")]
        public void AutoHatch()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            PromptEntityOptions peo =
                new PromptEntityOptions("\nSelect a closed boundary: ");

            peo.SetRejectMessage("\nObject must be a closed polyline.");
            peo.AddAllowedClass(typeof(Polyline), true);
            peo.AllowObjectOnLockedLayer = true;

            var result = ed.GetEntity(peo);

            if (result.Status != PromptStatus.OK)
                return;

            ObjectId boundaryId = result.ObjectId;

            // ⭐ UPDATED OPTIONS
            PromptKeywordOptions dirOptions =
                new PromptKeywordOptions("\nChoose line direction");

            dirOptions.Keywords.Add("Horizontal");
            dirOptions.Keywords.Add("Vertical");
            dirOptions.Keywords.Add("Both");

            dirOptions.AllowNone = false;

            var dirResult = ed.GetKeywords(dirOptions);

            if (dirResult.Status != PromptStatus.OK)
                return;

            string direction = dirResult.StringResult;

            PromptDoubleOptions spacingOptions =
                new PromptDoubleOptions("\nEnter spacing (drawing units): ");

            spacingOptions.AllowNegative = false;
            spacingOptions.AllowZero = false;

            var spacingResult = ed.GetDouble(spacingOptions);

            if (spacingResult.Status != PromptStatus.OK)
                return;

            double spacing = spacingResult.Value;

            HatchService.ProcessBoundary(boundaryId, direction, spacing);
        }
    }
}
