using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourceManagement.Data.Entities;

public class Course
{
    public int Id { get; set; }
    [MaxLength(200)]
    [Required]
    public string? Title { get; set; }
    [MaxLength(200)]
    [Required]
    public string? Topic { get; set; }
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
    [MaxLength(200)]
    [Required]
    public string? Author { get; set; }

    public ICollection<Lesson>? Lessons { get; set; }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasMany(e => e.Lessons)
        .WithOne(e => e.Course)
        .HasForeignKey(e => e.CourseId)
        .HasPrincipalKey(e => e.Id);
    }
}
