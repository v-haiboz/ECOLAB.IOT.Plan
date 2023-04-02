namespace ECOLAB.IOT.Plan.Common
{
    using System;

    public class CallerContextBase
    {
        public static T Get<T>() where T : class
        {
            var res = Container.GetService(typeof(T)) as T;
            return res;
        }

        static AsyncLocal<IServiceProvider> AsyncLocal { get; } = new AsyncLocal<IServiceProvider>();

        public static IServiceProvider Container
        {
            get
            {
                return AsyncLocal.Value;
            }
            set
            {
                AsyncLocal.Value = value;
            }
        }

    }
}