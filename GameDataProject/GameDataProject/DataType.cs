//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GameDataProject
{
    using System;
    using System.Collections.Generic;
    
    public partial class DataType
    {
        public DataType()
        {
            this.PlayerDatas = new HashSet<PlayerData>();
        }
    
        public int Id { get; set; }
        public string DataType1 { get; set; }
    
        public virtual ICollection<PlayerData> PlayerDatas { get; set; }
    }
}
