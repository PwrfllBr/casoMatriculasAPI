using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace casoMatriculasAPI.Models;

public partial class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudent { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(200)]
    public string? Email { get; set; }

    //Nav property for Enrollments
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
