using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Globalization;
using System.Collections;
using WebAPI03Application.Models;

using System.Web.Http.Cors;

namespace WebAPI03Application.Controllers
{
    //[EnableCors(origins: "http://localhost", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    [RoutePrefix("api/WealthPCustomize2")]

    public class WealthPCustomize2Controller : ApiController
    {
        /*
        [RoutePrefix("api/WealthPCustomize")]
        [Route("")]
        public ArrayList Get()
        {
            WealthPCustomizePersistance faa = new WealthPCustomizePersistance();
            if (faa == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return faa.getFundAAs();
        }
        */

        //[Route("{navDate}")]
        //public ArrayList Get(string navDate)
        //public ArrayList Post([FromBody]string cweight)
        public WealthPCustomize Post([FromBody]CWeight[] cweight)
        {
            WealthPCustomizePersistance2 wpc = new WealthPCustomizePersistance2();
            if (wpc == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return wpc.getWealthPCustomize(cweight);
        }

        /*
        // GET: api/WealthPCustomize
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/WealthPCustomize/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WealthPCustomize
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/WealthPCustomize/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/WealthPCustomize/5
        public void Delete(int id)
        {
        }
        */
    }
}
