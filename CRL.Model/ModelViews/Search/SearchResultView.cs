using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Search
{
    public enum SearchItemState { Current, Updated, CurrentUpdated, Deleted }
    
    public class SearchResultView
    {
        public bool IsSelected { get; set; }
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string RegistrationNo { get; set; }
        public SearchResultDebtor[] Debtors { get; set; }
        public SearchResultCollateral[] Collaterals { get; set; }
        public string Status { get; set; }

    }

    public class SearchResultDebtor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string IDNo { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public bool isIndividual { get; set; }
        public bool isTagged { get; set; }
        public bool isTaggedName { get; set; }
        public bool isTaggedId { get; set; }
        public bool isTaggedEmail { get; set; }
        public bool isTaggedDOB { get; set; }
        public SearchItemState SearchItemState { get; set; }
        
    }

    public class SearchResultCollateral
    {
        public int Id { get; set; }
        public string SerialNo { get; set; }
        public string Description { get; set; }
        public string SubTypeName { get; set; }
        public bool isTaggedSerialNo { get; set; }
        public bool istaggedDescription { get; set; }
        public bool isTagged { get; set; }
        public SearchItemState SearchItemState { get; set; }

    }
  
}
