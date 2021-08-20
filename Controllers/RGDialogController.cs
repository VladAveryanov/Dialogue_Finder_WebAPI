using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dialogue_Finder_WebAPI.Controllers
{
    [ApiController]
    public class RGDialogController: ControllerBase
    {
        public RGDialogController()
        {
            
        }

        /// <summary>
        /// This GET method returns dialogs by using client ID`s
        /// </summary>
        /// <param name="clients"></param>
        /// <returns>GUID of dialog</returns>
        [HttpGet]
        [Route("getDialog")]
        public IActionResult GetDialog ([FromQuery] Guid[] clients)
        {
            long hash = 1;
            foreach (var element in clients)
            {
                hash *= element.GetHashCode();
            }
            var dialogs = new RGDialogsClients().Init();

            // contains Hash of IDClients as Key and IDRGDialog as Value
            var dict1 = new Dictionary<long, Guid>();

            // contains IDRGDialog as key and Hash of IDClients as Value;
            var dict2 = new Dictionary<Guid, long>();

            foreach (var rgdialog in dialogs)
            { 
                if (dict1.Any() && dict2.ContainsKey(rgdialog.IDRGDialog))
                {
                    var rgCLientsHash = dict2[rgdialog.IDRGDialog];
                    dict1.Remove(rgCLientsHash);
                    var newHash = rgCLientsHash * rgdialog.IDClient.GetHashCode();
                    dict1[newHash] = rgdialog.IDRGDialog;
                    dict2[rgdialog.IDRGDialog] *= newHash;
                }
                dict1[rgdialog.IDClient.GetHashCode()] = rgdialog.IDRGDialog;
                dict2[rgdialog.IDRGDialog] = rgdialog.IDClient.GetHashCode();
            }
            Guid response = new Guid();
            if (dict1.ContainsKey(hash))
            {
                response = dict1[hash];
            }
            return Ok(response);
        }
    }
}
