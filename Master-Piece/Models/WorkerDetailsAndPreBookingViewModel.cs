﻿namespace Master_Piece.Models
{
    public class WorkerDetailsAndPreBookingViewModel
    {
        public User Employee { get; set; }
        public Booking NewBooking { get; set; } = new Booking();
        public int ServiceId { get; set; }
    }
}
