namespace ZYBW.ProjectMIS.Models.Dealer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;

    // The MetadataTypeAttribute identifies TbDealerInfoManageMetadata as the class
    // that carries additional metadata for the TbDealerInfoManage class.
    [MetadataTypeAttribute(typeof(V_FlowInfo.V_FlowInfoMetadata))]
    public partial class V_FlowInfo
    {
        // This class allows you to attach custom attributes to properties
        // of the TbDealerInfoManage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class V_FlowInfoMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private V_FlowInfoMetadata()
            {
            }

            /// <summary>
            /// 农药ID
            /// </summary>
            [Display(Name = "农药ID")]
            public Guid ID
            { set; get; }
            /// <summary>
            /// 名称
            /// </summary>
            [Display(Name = "名称")]
            public string name
            { set; get; }

            /// <summary>
            /// 出/入库
            /// </summary>
            [Display(Name = "出/入库")]
            public string InOrOut
            { set; get; }
            /// <summary>
            /// 数量
            /// </summary>
            [Display(Name = "数量")]
            public string Quantity
            { set; get; }
            /// <summary>
            /// 销售者
            /// </summary>
            [Display(Name = "销售者")]
            public string seller
            { set; get; }
            /// <summary>
            /// 购买者
            /// </summary>
            [Display(Name = "购买者")]
            public string buyer
            { set; get; }
            /// <summary>
            /// 时间
            /// </summary>
            [Display(Name = "时间")]
            public DateTime TheTime
            { set; get; }
        }
    }
}
