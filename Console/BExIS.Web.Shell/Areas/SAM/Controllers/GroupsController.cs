﻿using BExIS.Modules.Sam.UI.Models;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;

namespace BExIS.Modules.Sam.UI.Controllers
{
    public class GroupsController : Controller
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupName"></param>
        [HttpPost]
        public void AddUserToGroup(long userId, string groupName)
        {
            var userManager = new UserManager();
            var user = userManager.FindByIdAsync(userId).Result;

            userManager.AddToGroupAsync(user.Id, groupName);
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CreateGroupModel model)
        {
            if (!ModelState.IsValid) return PartialView("_Create", model);

            var groupManager = new GroupManager();
            var group = groupManager.CreateAsync(new Group()
            {
                Name = model.Name,
                Description = model.Description
            }).Result;

            return Json(new { success = true });
        }

        [GridAction]
        public ActionResult Groups_Select()
        {
            var groupManager = new GroupManager();
            var groups = groupManager.Groups.Select(GroupGridRowModel.Convert).ToList();

            return View(new GridModel<GroupGridRowModel> { Data = groups });
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupName"></param>
        [HttpPost]
        public void RemoveUserFromGroup(long userId, string groupName)
        {
            var userManager = new UserManager();
            var user = userManager.FindByIdAsync(userId).Result;

            userManager.RemoveFromGroupAsync(user.Id, groupName);
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ActionResult Update(long groupId)
        {
            var groupManager = new GroupManager();
            var group = groupManager.FindByIdAsync(groupId).Result;
            return View("_Update", UpdateGroupModel.Convert(group));
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(UpdateGroupModel model)
        {
            if (!ModelState.IsValid) return PartialView("_Update", model);

            var groupManager = new GroupManager();
            var group = groupManager.FindByIdAsync(model.Id).Result;

            if (group == null) return PartialView("_Update", model);

            group.Name = model.Name;
            group.Description = model.Description;

            groupManager.UpdateAsync(group);
            return Json(new { success = true });
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ActionResult Users(string groupName)
        {
            return PartialView("_Users", groupName);
        }

        [GridAction]
        public ActionResult Users_Select(string groupName = "")
        {
            var userManager = new UserManager();
            var users = new List<UserMembershipGridRowModel>();

            foreach (var user in userManager.Users)
            {
                users.Add(UserMembershipGridRowModel.Convert(user, groupName));
            }

            return View(new GridModel<UserMembershipGridRowModel> { Data = users });
        }

        /// <summary>
        /// ToDo: Documentation
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public JsonResult ValidateGroupName(string groupName, long groupId = 0)
        {
            var groupManager = new GroupManager();
            var group = groupManager.FindByNameAsync(groupName).Result;

            if (group == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (group.Id == groupId)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var error = string.Format(CultureInfo.InvariantCulture, "The group name already exists.");

                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}