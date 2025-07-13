using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
namespace WebCosmeticApp.Models;
public class Cosmetic
{
    private string name;

    
    public int Id { get; set; }

    public string Name { get => name; set => name = value; }

    public string Brand { get; set; }

    public string Category { get; set; }


    public string Picture { get; set; }



    public string Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    [NotMapped]
    public IFormFile ImageFile { get; set; }
}
