using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;

namespace NailsChekin.Models.Reports
{
    public static class SinglePageHelper
    {
        public static void GenerateSinglePageReport(XtraReport report)
        {
            float sumHeight = 0;

            //For TEST
            report.PrintingSystem.PageSettings.AssignDefaultPageSettings();

            report.CreateDocument();

            XtraPageSettingsBase pageSettings = report.PrintingSystem.PageSettings;
            XtraPageSettingsBase.ApplyPageSettings(pageSettings, PaperKind.Custom,
                new Size(pageSettings.Bounds.Width, pageSettings.Bounds.Height * report.Pages.Count),
                pageSettings.Margins, pageSettings.MinMargins, pageSettings.Landscape);

            NestedBrickIterator iterator = new NestedBrickIterator(report.Pages[0].InnerBricks);
            while (iterator.MoveNext())
                if (iterator.CurrentBrick is VisualBrick)
                {
                    VisualBrick brick = (VisualBrick)iterator.CurrentBrick;
                    float bottomPos = brick.Rect.Bottom;

                    if (bottomPos > sumHeight)
                        sumHeight = bottomPos;
                }

            sumHeight = GraphicsUnitConverter.Convert(sumHeight, GraphicsUnit.Document, GraphicsUnit.Inch) * 100;

            int totalPageHeight = pageSettings.Margins.Top + pageSettings.Margins.Bottom + Convert.ToInt32(sumHeight);

            XtraPageSettingsBase.ApplyPageSettings(pageSettings, PaperKind.Custom,
                new Size(pageSettings.Bounds.Width, totalPageHeight),
                pageSettings.Margins, pageSettings.MinMargins, pageSettings.Landscape);

            //report.RollPaper = true;
            //report.PaperKind = PaperKind.Custom;
            //report.PageSize = new Size(pageSettings.Bounds.Width, totalPageHeight);
        }

    }
}
