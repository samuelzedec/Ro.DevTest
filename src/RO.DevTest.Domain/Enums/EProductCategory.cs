using System.ComponentModel;

namespace RO.DevTest.Domain.Enums;

public enum EProductCategory
{
    [Description("Eletrônicos")]
    Electronics = 1,
    
    [Description("Roupas e Acessórios")]
    Clothing = 2,
    
    [Description("Casa e Cozinha")]
    HomeAndKitchen = 3,
    
    [Description("Livros")]
    Books = 4,
    
    [Description("Esportes e Ar Livre")]
    SportsAndOutdoors = 5,
    
    [Description("Beleza e Cuidados Pessoais")]
    BeautyAndPersonalCare = 6,
    
    [Description("Brinquedos e Jogos")]
    ToysAndGames = 7,
    
    [Description("Automotivo")]
    Automotive = 8,
    
    [Description("Material de Escritório")]
    OfficeSupplies = 9,
    
    [Description("Alimentos e Bebidas")]
    FoodAndGrocery = 10
}