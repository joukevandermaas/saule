using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization.ErrorRenderers
{
    internal class DbEntityValidationExceptionRenderer : ExceptionRenderer
    {
        private DbEntityValidationException _exception;


        public DbEntityValidationExceptionRenderer(DbEntityValidationException validationException)
        {
            _exception = validationException;
        }

        public override JArray ToJObject()
        {
            var result = new JArray();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var dbError in _exception.EntityValidationErrors)
            {
                foreach (var dbErrorEntry in dbError.ValidationErrors)
                {
                    var error = AsError(dbError, dbErrorEntry);
                    result.Add(error);
                }

            }


            return result;
        }


        private object AsError(DbEntityValidationResult dbError, DbValidationError dbErrorEntry)
        {
            var status = "422";
            var detail = dbErrorEntry.ErrorMessage;
            var source = new { pointer = "/data/attributes/" + dbErrorEntry.PropertyName };

            return new { status = status, detail = detail, source = source };
        }
    }
}
