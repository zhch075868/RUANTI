using Kingdee.BOS;
using Kingdee.BOS.JSON;
using Kingdee.BOS.ServiceFacade.KDServiceFx;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.WebApi.ServicesStub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.WebApi
{
    public class GetBillNo:AbstractWebApiBusinessService
    {
        private Context context;

        public GetBillNo(KDServiceContext context)
            : base(context)
        {
        }

        public JSONObject GetBillNoByFormId(string parameter)
        {
            JSONObject retJson = new JSONObject();
            context = this.KDContext.Session.AppContext;
            bool conBool = Utils.isCloudSSO(this.KDContext.Session.AppContext);
            if (conBool == false)
            {
                retJson = new JSONObject();
                retJson.Add("IsSuccess", "0");
                retJson.Add("Message", "请先登陆验证!");
                return retJson;
            }

            JSONObject parameterJson = JSONObject.Parse(parameter);
            string formId = Convert.ToString(parameterJson.GetString("FromId"));
            string ruleId = Convert.ToString(parameterJson.GetString("RuleId"));
            int count = parameterJson.GetInt("Count");
            var billNos = BusinessDataServiceHelper.GetListBillNO(this.context, formId, count, ruleId);

            retJson = new JSONObject();
            retJson.Add("IsSuccess", "1");
            retJson.Add("Message", "获取成功!");
            retJson.Add("billNo", billNos);
            return retJson;

            return retJson;
        }
    }
}
