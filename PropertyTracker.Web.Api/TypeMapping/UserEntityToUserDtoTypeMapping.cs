﻿
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using AutoMapper;
using PropertyTracker.Web.Api.AutoMapping;
using PropertyTracker.Web.Entity.Models;

namespace PropertyTracker.Web.Api.TypeMapping
{
    public class UserEntityToUserDtoTypeMapping : IAutoMapperTypeMapping
    {
        public void Configure()
        {
            Mapper.CreateMap<Entity.Models.User, Dto.Models.User>()
                .ForMember(dm => dm.Password, x => x.Ignore()) // don't want to send password
                 .ForMember(dm => dm.PhotoUrl, x => x.Ignore()) 
                //.ForMember(dm => dm.PhotoUrl, em => em.ResolveUsing<PhotoResolver>()) - not working, let controllers handle this
                
                // Properties no longer part of User DTO
                //.ForMember(dm => dm.Properties, em => em.ResolveUsing<PropertiesResolver>()) // Method 1
                //.ForMember(dm => dm.Properties, em => em.MapFrom(user => user.Properties.Select(p => p.Id).ToList())) // Method 2 - use MapFrom with Linq to create idlist
                ; 
        }


        // This is one-way to flatten a User's properties down to list
        public class PropertiesResolver : ValueResolver<Entity.Models.User, List<int>>  
        {
            protected override List<int> ResolveCore(Entity.Models.User user)
            {
                List<int> idList = user.Properties.Select(p => p.Id).ToList();
                return idList;
            }
        }

        // Can't resolve URLs here - we don't know the URL - 
        public class PhotoResolver : ValueResolver<Entity.Models.User, string>
        {
            protected override string ResolveCore(Entity.Models.User user)
            {
               // This is not working - 
               // UrlHelper Url = (UrlHelper)HttpContext.Current.Items["Url"];
               // return "http://newphoto/" + user.Id; //Url.Link("GetUserRoute", user.Id);
               // return Url.Link("GetUserRoute", user.Id);
                return "some_link";
            }
        }
    }
}