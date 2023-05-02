using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using Trilhas.Data.Model.Exceptions;

namespace Trilhas.Controllers
{
    public abstract class DefaultController : Controller
    {
        protected UserManager<IdentityUser> _userManager;

        public DefaultController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string RecuperarUsuarioId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return String.Empty;
            }

            return _userManager.GetUserId(User);
        }

        public string RecuperarUsuarioEmail()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return String.Empty;
            }

            var claim = User.FindFirst("email");

            return claim != null ? claim.Value : String.Empty;
        }

        public bool UsuarioGestor()
        {
            return User.IsInRole("Gestor");
        }

        protected ActionResult JsonFormResponse(Object data)
        {
            Response.StatusCode = 200;

            return Json(data);
        }

        protected ActionResult JsonErrorFormResponse(TrilhasException ex)
        {
            List<JsonValidationError> errorList = null;

            if (!ModelState.IsValid)
            {
                errorList = PreencherErrosDeValidacao();
                Response.StatusCode = 400;
            }
            else
            {
                Response.StatusCode = 500;
            }

            var response = new JsonResponse()
            {
                Message = ex.Message,
                Errors = errorList
            };

            return Json(response);
        }

        protected ActionResult JsonErrorFormResponse(Exception ex, String message = "")
        {
            List<JsonValidationError> errorList = null;

            if (!ModelState.IsValid)
            {
                errorList = PreencherErrosDeValidacao();
                Response.StatusCode = 400;
            }
            else
            {
                Response.StatusCode = 500;
            }

            var response = new JsonResponse()
            {
                Message = message,
                InternalMessage = ex.Message,
                Errors = errorList
            };

            return Json(response);
        }

        private List<JsonValidationError> PreencherErrosDeValidacao()
        {
            List<JsonValidationError> errorList = new List<JsonValidationError>();

            foreach (var key in ModelState.Keys)
            {
                if (ModelState.TryGetValue(key, out ModelStateEntry entry))
                {
                    foreach (var error in entry.Errors)
                    {
                        errorList.Add(new JsonValidationError()
                        {
                            Key = key,
                            Message = error.ErrorMessage
                        });
                    }
                }
            }

            return errorList;
        }
    }

    public class JsonResponse
    {
        //public string Type { get; set; }
        public string InternalMessage { get; set; }
        public string Message { get; set; }
        public IEnumerable<JsonValidationError> Errors { get; set; }

        public JsonResponse()
        {
            Errors = new List<JsonValidationError>();
        }
    }

    public class JsonValidationError
    {
        public string Key { get; set; }
        public string Message { get; set; }
    }
}