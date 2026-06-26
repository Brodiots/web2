using System;

namespace TravelPlanner.Common.DTOs.Trip
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal EstimatedBudget { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateTripDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal EstimatedBudget { get; set; }
        public Guid UserId { get; set; }
    }
}