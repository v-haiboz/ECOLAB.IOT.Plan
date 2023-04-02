namespace ECOLAB.IOT.Plan.Entity.Entities.SqlServer
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute: Attribute
    {
        private PrimaryKeyAttribute()
        {

        }

        private string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public PrimaryKeyAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// primary kkey
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}
