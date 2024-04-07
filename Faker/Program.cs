using FakerProj.Entities;
using FakerProj.FakerLib;


var config = new FakerConfig();
var faker = new Faker(config);
config.Add<B, int, CustomIntGenerator>(b => b.Id);