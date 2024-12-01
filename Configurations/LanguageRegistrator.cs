namespace stockwatch.Configurations
{
    public static class LanguageRegistrator
    {
        public static void SetLanguage()
        {
            //get lang as "en"
            var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            //toggle lang
            if (lang == "vi")
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("vi-VN");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
        }
    }
}
