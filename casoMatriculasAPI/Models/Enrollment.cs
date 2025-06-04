using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace casoMatriculasAPI.Models;

public partial class Enrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEnrollment { get; set; }

    [Required]
    public int IdStudent { get; set; }

    [Required]
    public int IdCourse { get; set; }

    // "Activa", "Finalizada" "Cancelada"
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Activa"; // "Activa" by default on creation

    [Required]
    public DateTime EnrollmentDate { get; set; }

    // Nav properties for foreign keys
    [ForeignKey("IdStudent")]
    public virtual Student Student { get; set; } = null!;
    [ForeignKey("IdCourse")]
    public virtual Course Course { get; set; } = null!;

}
