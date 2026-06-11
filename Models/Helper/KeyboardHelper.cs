
namespace NailsChekin.Models.Helper
{
    public static class KeyboardHelper
    {
        public static string HandleKeyPress(string currentText, string amtDueText, string buttonText)
        {
            double tender = Utilitys.getTotalAmount(currentText);
            double amtDue = Utilitys.getTotalAmount(amtDueText);

            if (tender <= 0 || tender == amtDue )
                currentText = "";

            switch (buttonText)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "*":
                case ".":
                    currentText += buttonText;
                    break;

                case "C":
                    currentText = "";
                    break;

                case "<<":
                    currentText = currentText.Length > 1
                        ? currentText.Substring(0, currentText.Length - 1)
                        : "";
                    break;
            }

            return (currentText.StartsWith("$") ? "" : "$") + currentText;
        }
    }
}
