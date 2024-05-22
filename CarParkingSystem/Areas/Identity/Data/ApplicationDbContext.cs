#nullable disable
using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarParkingSystem.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) 
    {
    }
    public DbSet<Permit> Permits { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }    
    public DbSet<Bay> Bays { get; set; }
    public DbSet<UserLicense> Licenses {  get; set; }  
    public DbSet<PermitPayment> PermitPayment { get; set; } 
    public DbSet<OrderDetails> OrderDetails {  get; set; }  
    public DbSet<ParkingBay> ParkingBays {  get; set; }
    public DbSet<Reservations> Reservations {  get; set; }  
    public DbSet<ParkingOrderDetails> ParkingOrderDetails {  get; set; }    
    public DbSet<CheckIn> CheckIns { get; set; }
    public DbSet<CheckOut> checkOuts {  get; set; }
    public DbSet<Visitation> Visitations {  get; set; } 
    public DbSet<Feedback> Feedbacks {  get; set; }
    public DbSet<ParkingPayment> parkingPayments {  get; set; }
    public DbSet<GuardSchedule> GuardSchedules {  get; set; }   
    public DbSet<BlackListDriver> BlackListDrivers {  get; set; }   
    public DbSet<EventPayment> EventPayments { get; set; }
    public DbSet<StudentEvent> StudentEvents {  get; set; } 
    public DbSet<ViolationImages> ViolationImages {  get; set; }    
    public DbSet<StudentViolation> StudentViolations {  get; set; } 
    public DbSet<EventCheckIn> EventCheckIns {  get; set; } 
    public DbSet<ViolationPayment> ViolationPayments {  get; set; } 
    public DbSet<ViolationAppeal> ViolationAppeals {  get; set; }
    public DbSet<FaultReported> FaultReported { get; set; }
    public DbSet<Fault> Fault { get; set; }
    public DbSet<TechnicianReport> TechnicianReport { get; set; }
    public DbSet<Noticeboard> Noticeboard { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
       
    }

}
