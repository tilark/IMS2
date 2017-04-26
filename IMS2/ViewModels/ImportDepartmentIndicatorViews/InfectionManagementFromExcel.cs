using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 感染管理部门
    /// </summary>
    public class InfectionManagementFromExcel
    {
        //[Display(Name = "医院感染新发病人数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "医院感染总例次")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData2")]
        [Required]
        public virtual string Data2 { get; set; }

        //[Display(Name = "医院感染漏报例次")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData3")]
        [Required]
        public virtual string Data3 { get; set; }

        //[Display(Name = "使用非限制级抗菌药物前病原学送检例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData4")]
        [Required]
        public virtual string Data4 { get; set; }

        //[Display(Name = "使用非限制级抗菌药物总例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData5")]
        [Required]
        public virtual string Data5 { get; set; }

        //[Display(Name = "使用限制级抗菌药物前病原学送检例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData6")]
        [Required]
        public virtual string Data6 { get; set; }

        //[Display(Name = "使用限制级抗菌药物总例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData7")]
        [Required]
        public virtual string Data7 { get; set; }

        //[Display(Name = "使用特殊级抗菌药物前病原学送检例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData8")]
        [Required]
        public virtual string Data8 { get; set; }

        //[Display(Name = "使用特殊级抗菌药物总例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData9")]
        [Required]
        public virtual string Data9 { get; set; }

        //[Display(Name = "正确的洗手人数【抽查】")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData10")]
        [Required]
        public virtual string Data10 { get; set; }

        //[Display(Name = "洗手总人数【抽查】")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData11")]
        [Required]
        public virtual string Data11 { get; set; }

        //[Display(Name = "实际实施手卫生次数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData12")]
        [Required]
        public virtual string Data12 { get; set; }

        //[Display(Name = "应实施手卫生次数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData13")]
        [Required]
        public virtual string Data13 { get; set; }

        //[Display(Name = "手术部位感染上报例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData14")]
        [Required]
        public virtual string Data14 { get; set; }

        //[Display(Name = "I类切口手术部位感染例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData15")]
        [Required]
        public virtual string Data15 { get; set; }

        //[Display(Name = "血管内导管相关血流感染发病例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData16")]
        [Required]
        public virtual string Data16 { get; set; }

        //[Display(Name = "血管内导管留置总天数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData17")]
        [Required]
        public virtual string Data17 { get; set; }

        //[Display(Name = "呼吸机相关肺炎发病例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData18")]
        [Required]
        public virtual string Data18 { get; set; }

        //[Display(Name = "呼吸机使用总天数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData19")]
        [Required]
        public virtual string Data19 { get; set; }

        //[Display(Name = "导尿管相关泌尿系感染发病例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData20")]
        [Required]
        public virtual string Data20 { get; set; }

        //[Display(Name = "导尿管使用总天数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData21")]
        [Required]
        public virtual string Data21 { get; set; }

        //[Display(Name = "医院感染现患人数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData22")]
        [Required]
        public virtual string Data22 { get; set; }

        //[Display(Name = "医院感染现患检测时间住院人数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "InfectionManagementData23")]
        [Required]
        public virtual string Data23 { get; set; }

       
    }
}