namespace stockwatch.Configurations
{
    public static class LanguageRegister
    {
        private const string VnIsoLanguageName = "vi";
        private const string VnCultureInfoName = "vi-VN";
        private const string EnCultureInfoName = "en-US";

        public static void SetLanguage()
        {
            var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            if (lang == VnIsoLanguageName)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(VnCultureInfoName);
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(VnCultureInfoName);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(EnCultureInfoName);
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(EnCultureInfoName);
            }
        }
    }
}
