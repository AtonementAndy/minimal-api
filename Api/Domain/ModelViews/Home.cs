namespace minimal_api.Domain.ModelViews
{
    public struct Home
    {
        public readonly string Message { get => "Welcome to the Vehicles - Minimal API."; }
        public readonly string Doc { get => "/swagger"; }
    }
}
