﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using StudentManage.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolManagement.Controllers
{
    public class AccountController : Controller
    {
        private MyDBContext db;
        private UserManager<Account> userManager;
        private RoleManager<Role> roleManager;
        private SignInManager<Account, string> signInManager;

        // GET: Account
        public AccountController()
        {
            db = new MyDBContext();
            UserStore<Account> userStore = new UserStore<Account>(db);
            userManager = new UserManager<Account>(userStore);
            RoleStore<Role> roleStore = new RoleStore<Role>(db);
            roleManager = new RoleManager<Role>(roleStore);
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind(Include = "username,password")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var account = await userManager.FindAsync(login.username, login.password);
                if (account != null)
                {
                    signInManager = new SignInManager<Account, string>(userManager, Request.GetOwinContext().Authentication);
                    await signInManager.SignInAsync(account, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return View(login);
                }

            }
            return View(login);
            
        }
        [Authorize]
        public ActionResult AddAccount()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAccount([Bind(Include = "firstname,lastname,username,roll_number,password,email,phoneNumber,address,birthday,gender")] AddAcountViewModel accountView)
        {
            string password = accountView.password;
            if (ModelState.IsValid)
            {

                Account account = new Account()
                {
                    Firstname = accountView.firstname,
                    Lastname = accountView.lastname,
                    UserName = accountView.username,
                    Email = accountView.email,
                    PhoneNumber = accountView.phoneNumber,
                    gender = accountView.gender,
                    roll_number= accountView.roll_number,
                    address = accountView.address,
                    birthday  = accountView.birthday
                };
                var result = await userManager.CreateAsync(account, password);
                if (result.Succeeded)
                {
                    return View("CreateAccountSuccess");
                }
                else
                {
                    return View(accountView);
                }
            }
            return View(accountView);

           
        }

        public async Task<ActionResult> AddRole()
        {
            Role role = new Role()
            {
                Name = "Admin"
            };
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return View("CreateAccountSuccess");
            }
            else
            {
                return View("CreateAccountFails");
            }
        }
    }
}