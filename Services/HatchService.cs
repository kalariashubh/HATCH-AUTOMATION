using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace hatch_automation.Services
{
    public static class HatchService
    {
        public static void ProcessBoundary(ObjectId boundaryId, string direction, double spacing)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            List<LineData> horizontalLines = new List<LineData>();
            List<LineData> verticalLines = new List<LineData>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Polyline boundary =
                    tr.GetObject(boundaryId, OpenMode.ForWrite) as Polyline;

                if (boundary == null)
                {
                    ed.WriteMessage("\nOnly closed polylines supported.");
                    return;
                }

                if (!boundary.Closed)
                    boundary.Closed = true;

                BlockTable bt =
                    tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord btr =
                    tr.GetObject(bt[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                Extents3d ext = boundary.GeometricExtents;

                double minX = ext.MinPoint.X;
                double maxX = ext.MaxPoint.X;
                double minY = ext.MinPoint.Y;
                double maxY = ext.MaxPoint.Y;

                // ⭐ Horizontal
                if (direction == "Horizontal" || direction == "Both")
                {
                    ed.WriteMessage("\n\n--- Generating Horizontal Lines ---");

                    for (double y = minY; y <= maxY; y += spacing)
                    {
                        Line testLine = new Line(
                            new Point3d(minX - 1000, y, 0),
                            new Point3d(maxX + 1000, y, 0));

                        Point3dCollection pts = new Point3dCollection();

                        boundary.IntersectWith(
                            testLine,
                            Intersect.OnBothOperands,
                            pts,
                            System.IntPtr.Zero,
                            System.IntPtr.Zero);

                        CreateInteriorLines(pts, btr, tr, horizontalLines, ed, "H");
                    }
                }

                // ⭐ Vertical
                if (direction == "Vertical" || direction == "Both")
                {
                    ed.WriteMessage("\n\n--- Generating Vertical Lines ---");

                    for (double x = minX; x <= maxX; x += spacing)
                    {
                        Line testLine = new Line(
                            new Point3d(x, minY - 1000, 0),
                            new Point3d(x, maxY + 1000, 0));

                        Point3dCollection pts = new Point3dCollection();

                        boundary.IntersectWith(
                            testLine,
                            Intersect.OnBothOperands,
                            pts,
                            System.IntPtr.Zero,
                            System.IntPtr.Zero);

                        CreateInteriorLines(pts, btr, tr, verticalLines, ed, "V");
                    }
                }

                JsonService.Export(horizontalLines, verticalLines);

                ed.WriteMessage(
                    $"\n\nSUCCESS ✅ Horizontal: {horizontalLines.Count} | Vertical: {verticalLines.Count}");

                tr.Commit();
            }
        }

        private static void CreateInteriorLines(
            Point3dCollection pts,
            BlockTableRecord btr,
            Transaction tr,
            List<LineData> lines,
            Editor ed,
            string prefix)
        {
            if (pts.Count < 2)
                return;

            for (int i = 0; i < pts.Count - 1; i += 2)
            {
                Line inside = new Line(pts[i], pts[i + 1]);

                btr.AppendEntity(inside);
                tr.AddNewlyCreatedDBObject(inside, true);

                double length = inside.Length;
                int lineNumber = lines.Count + 1;

                ed.WriteMessage(
                    $"\n{prefix}-Line {lineNumber} Length = {length:F2}");

                lines.Add(new LineData
                {
                    LineNumber = lineNumber,
                    Length = length
                });
            }
        }
    }
}
