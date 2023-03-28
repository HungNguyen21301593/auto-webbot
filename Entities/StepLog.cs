using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class StepLog: CreateAbstract
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(24)")]
        public StepType Type { get; set; }
        [Column(TypeName = "varchar(24)")]
        public Result Result { get; set; }
        public string Message { get; set; }
        public Post Post { get; set; }
    }

    public enum Result
    {
        Success,
        Failed,
        Skip
    }

    public enum StepType
    {
        ReadDynamicText,
        ReadTitle,
        DownloadPics,
        ReadCategories,
        ReadAdDescription,
        ReadTags,
        ReadAddress,
        ReadLocation,
        ReadPrice,
        ReadCompany,
        ReadTypes,
        ReadCarYear,
        ReadCarKm,
        ReadPhoneNumber,

        SearchAdBeforeDelete,
        SubmitDelete,

        InputTitle,
        SelectCategories,
        InputDescription,
        SelectAdtype,
        SelectForSaleBy,
        SelectMoreInfo,
        SelectFulfillment,
        SelectPayment,
        SelectTags,
        TryInputPicture,
        InputLocation,
        InputAddress,
        InputPrice,
        InputCompany,
        InputPhone,
        InputDynamicInputs,
        InputCarYear,
        InputCarKm,
        SelectBasicPakage,
        ActiveTermAndCondition,
        SubmitPost
    }
}
