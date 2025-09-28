namespace StartupExplained
{
    // Options pattern POCO
    public sealed class MyFeatureOptions
    {
        public const string SectionName = "MyFeature";
        public bool Enabled { get; set; } = true;
        public int RefreshSeconds { get; set; } = 30;
    }
}


