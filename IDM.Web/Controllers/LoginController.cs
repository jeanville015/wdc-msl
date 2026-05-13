using IDM.Service.User.Interface;
using IDM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IDM.DTO.User;
using System.DirectoryServices;

namespace IDM.Web.Controllers
{
    public class LoginController : BaseController
    {
        public readonly IAccountService _accountService;

        public LoginController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: /Account/Login?ReturnUrl=...
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Login(string returnUrl)
        {
            // Stash it in ViewBag so the POST form can pass it along
            ViewBag.ReturnUrl = returnUrl;

            var model = new LoginViewModel { UserName = "" };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel viewModel, string returnUrl)
        {
            try
            {
                var account = await _accountService.GetByAccountName(viewModel.UserName);
                
                if (account != null)
                {
                    if(!CheckActiveDirectory(viewModel))
                    {
                        ViewBag.ReturnUrl = returnUrl;
                        ViewData["ErrorMessage"] = "AD Account error please check your AD and password if correct!";
                        return View(viewModel);
                    }

                    await _accountService.UpdateLastLoginAsync(account.Id);
                    
                    System.Diagnostics.Debug.WriteLine($"Before SetAuthCookie - IsAuthenticated: {User.Identity.IsAuthenticated}");
                    
                    FormsAuthentication.SetAuthCookie(account.User_Id, false);
                    
                    System.Diagnostics.Debug.WriteLine($"After SetAuthCookie - IsAuthenticated: {User.Identity.IsAuthenticated}");
                    
                    Session["UserId"] = account.User_Id;
                    Session["AccountId"] = account.Id;
                    Session["UserRole"] = account.User_Role;
                    Session["UserGroup"] = account.User_Group;
                    Session["FullName"] = viewModel.FullName;

                    System.Diagnostics.Debug.WriteLine($"Session set - UserId: {Session["UserId"]}");

                    // ✅ KEY: validate then redirect back to original link
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home", new { accountId = account.Id, userName = account.User_Id });
                }
                else
                {
                    ViewData["ErrorMessage"] = "Account not found or inactive.";
                    return View(viewModel);
                }
            }
            catch(Exception)
            {
                ViewData["ErrorMessage"] = "An error occurred during login. Please try again.";
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            
            Session.Clear();
            Session.Abandon();
            
            return RedirectToAction("Login", "Login");
        }

        public bool CheckActiveDirectory(LoginViewModel u)
        {
            bool isTrue = false;

            // Validate password is not empty or null
            if (string.IsNullOrEmpty(u.Password))
            {
                return false;
            }

            using (DirectoryEntry adsEntry = new DirectoryEntry("LDAP://DC=ad,DC=shared", u.UserName, u.Password))
            {
                using (DirectorySearcher adsSearcher = new DirectorySearcher(adsEntry))
                {
                    adsSearcher.Filter = "(sAMAccountName=" + u.UserName + ")";

                    try
                    {
                        SearchResult adsSearchResult = adsSearcher.FindOne();
                        
                        // Only set isTrue to true if we actually found a result
                        if (adsSearchResult != null)
                        {
                            isTrue = true;
                            u.FullName = adsSearchResult.Properties["displayName"][0].ToString();
                            //u.Department = adsSearchResult.Properties["department"][0].ToString();
                            u.Email = adsSearchResult.Properties["mail"][0].ToString();
                            //u.Company = adsSearchResult.Properties["company"][0].ToString();
                            //u.Address = adsSearchResult.Properties["streetaddress"][0].ToString();
                            //u.Country = adsSearchResult.Properties["co"][0].ToString();
                            //u.PhoneNumber = adsSearchResult.Properties["telephonenumber"][0].ToString();
                            //u.Title = adsSearchResult.Properties["title"][0].ToString();
                            string managerName = adsSearchResult.Properties["manager"][0].ToString().Split(',')[0].Split('=')[1];
                            string[] nameParts = managerName.Split(' ');
                            string simplifiedName = string.Join(" ", nameParts.Take(nameParts.Length - 1));
                            //u.Manager = simplifiedName;

                            //u.ShortFullName = FormatName(u.FullName, false);
                            //u.Avatar = FormatName(u.FullName, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        //db.Log(ex.ToString());
                        //strerr = ex.Message;
                        System.Diagnostics.Debug.WriteLine($"AD Authentication failed: {ex.Message}");
                        return false; // Explicitly return false on any authentication exception
                    }
                    finally
                    {
                        adsEntry.Close();
                    }
                }
            }
            return isTrue;
        }

        public static string FormatName(string fullName, bool isAvatar)
        {
            string[] nameParts = fullName.Split(' ');

            if (nameParts.Length >= 2)
            {
                string firstInitial = nameParts[0][0].ToString();
                string lastName = nameParts.Last();

                if (lastName == "(CW)" && nameParts.Length > 1)
                    lastName = nameParts[nameParts.Length - 2];

                if (isAvatar)
                    return firstInitial + lastName[0];
                else
                    return firstInitial + ". " + lastName;
            }

            return fullName;
        }
    }
}