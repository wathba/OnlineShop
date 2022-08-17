using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using AutoMapper;

namespace API.RequestHelper
{
 public class MappingProfiles : Profile
 {
  public MappingProfiles()
  {
   CreateMap<ProductCreatedDto, Product>();
    CreateMap< ProductUpdatedDto, Product>();
  }
 }
}