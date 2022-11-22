using BookingRentals.Calendar;
using Moq;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Calendar;
using Xunit;
using Xunit.Abstractions;

namespace VacationRantal.Tests
{
    public class CalendarServiceTests
    {
        readonly ITestOutputHelper _output;
        ICalendar _service;
        ICalendarEngine _engine;
        Mock<IRepository<IdentifiedCalendarBookingItem>> _repository;
        public CalendarServiceTests(ITestOutputHelper output)
        {
            _repository = new Mock<IRepository<IdentifiedCalendarBookingItem>>();
            _service = new CalendarService(_repository.Object);
            _engine = _service as ICalendarEngine;
            _output = output;
        }

        [Theory]
        [InlineData(1, 3, 1, 100, new int[] { 1, 2, 3 })]
        public void TestGenerateCalendarBookingItems(int rentalId, int nights, int preparationDays, int bookingId, IEnumerable<int> units)
        {
            var startDate = System.DateTime.Today;
            var result = _engine.GenerateCalendarBookingItems(
                startDate: startDate,
                rentalId: rentalId,
                nights: nights,
                preparationDays: preparationDays,
                units: units,
                bookingId: bookingId);

            Assert.Equal((nights + preparationDays) * units.Count(), result.Count());
            Assert.Equal(nights * units.Count(), result.Count(x => x.BookingId == bookingId));
            Assert.Equal(preparationDays * units.Count(), result.Count(x => x.PreparationAfterBookingId == bookingId));

            Assert.Equal((nights + preparationDays) * units.Count(), result.Count(x => x.RentalId == rentalId));
            units.ToList().ForEach(unit => Assert.Equal(nights + preparationDays, result.Count(item => item.Unit == unit)));
            result.ToList().ForEach(item => Assert.Equal($"{item.Date:yyyy-MM-dd}|{rentalId}|{item.Unit}", item.Key));
            Assert.Equal(startDate.Date, result.OrderBy(x => x.Date).First().Date.Date);
            Assert.Equal(startDate.AddDays(nights + preparationDays - 1).Date, result.OrderBy(x => x.Date).Last().Date.Date);
        }

        [Theory]
        [InlineData(1, 1, 3)]
        public void TestGetPreparationDaysToAdd_RentalIdNotExistsOrNoPreparationDays(int rentalId, int oldPreparationDays, int newPreparationDays)
        {
            _repository.Setup(repo => repo.Get(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(new IdentifiedCalendarBookingItem[] { });
            var result = _engine.GetPreparationDaysToAdd(rentalId, newPreparationDays - oldPreparationDays);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1, 1, 3)]
        [InlineData(1, 3, 10)]
        [InlineData(1, 12, 20)]
        public void TestGetPreparationDaysToAdd_ReturnPreparationDays(int rentalId, int oldPreparationDays, int newPreparationDays)
        {
            var startDate = System.DateTime.Today;
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(Enumerable.Range(0, oldPreparationDays).Select(day => new IdentifiedCalendarBookingItem() {
                RentalId = rentalId,
                Date = startDate.AddDays(day),
                Unit = 1,
                PreparationAfterBookingId = 100,
                Id = $"{startDate.AddDays(day):yyyy-MM-dd}|{rentalId}|{1}",
                BookingId = null
            }));
            var result = _engine.GetPreparationDaysToAdd(rentalId, newPreparationDays - oldPreparationDays);
            Assert.Equal(newPreparationDays - oldPreparationDays, result.Count());
            Assert.Equal(startDate.AddDays(newPreparationDays - 1).Date, result.OrderBy(x => x.Date).Last().Date.Date);
        }

        [Theory]
        [InlineData(1, 3, 1)]
        public void TestGetPreparationDaysKeysToRemove_RentalIdNotExistsOrNoPreparationDays(int rentalId, int oldPreparationDays, int newPreparationDays)
        {
            _repository.Setup(repo => repo.Get(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(new IdentifiedCalendarBookingItem[] { });
            var result = _engine.GetPreparationDaysToAdd(rentalId, newPreparationDays - oldPreparationDays);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1, 3, 1)]
        [InlineData(1, 10, 3)]
        [InlineData(1, 20, 12)]
        public void TestGetPreparationDaysKeysToRemove_ReturnPreparationDayKeys(int rentalId, int oldPreparationDays, int newPreparationDays)
        {
            var startDate = System.DateTime.Today;
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(Enumerable.Range(0, oldPreparationDays).Select(day => new IdentifiedCalendarBookingItem()
            {
                RentalId = rentalId,
                Date = startDate.AddDays(day),
                Unit = 1,
                PreparationAfterBookingId = 100,
                Id = $"{startDate.AddDays(day):yyyy-MM-dd}|{rentalId}|{1}",
                BookingId = null
            }));
            var result = _engine.GetPreparationDaysKeysToRemove(rentalId, oldPreparationDays - newPreparationDays);
            Assert.Equal(oldPreparationDays - newPreparationDays, result.Count());
        }

        [Theory]
        [InlineData(1, new int[] { 1 }, 3, 3)]
        [InlineData(1, new int[] { 1, 2 }, 3, 3)]
        public void TestGetAvailableUnits_Unit1AndUnit2Reserved_1Available(int nights, int[] unitsReserved, int rentalUnits, int reservedNights)
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = 1, Units = rentalUnits };
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(unitsReserved.SelectMany(unit => Enumerable.Range(0, reservedNights).Select(day => new IdentifiedCalendarBookingItem()
            {
                RentalId = rentalItem.Id,
                Date = startDate.AddDays(day),
                Unit = unit,
                PreparationAfterBookingId = day >= reservedNights ? 100 : (int?)null,
                Id = $"{startDate.AddDays(day):yyyy-MM-dd}|{rentalItem.Id}|{unit}",
                BookingId = day < reservedNights ? 100 : (int?)null
            })));
            var result = _service.GetUnitsAvailable(rentalItem, startDate, nights);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(1, new int[] { 1, 2, 3 }, 3, 3)]
        public void TestGetAvailableUnits_AllUnitsReserved_NotAvailable(int nights, int[] unitsReserved, int rentalUnits, int reservedNights)
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = 1, Units = rentalUnits };
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(unitsReserved.SelectMany(unit => Enumerable.Range(0, reservedNights).Select(day => new IdentifiedCalendarBookingItem()
            {
                RentalId = rentalItem.Id,
                Date = startDate.AddDays(day),
                Unit = unit,
                PreparationAfterBookingId = day >= reservedNights ? 100 : (int?)null,
                Id = $"{startDate.AddDays(day):yyyy-MM-dd}|{rentalItem.Id}|{unit}",
                BookingId = day < reservedNights ? 100 : (int?)null
            })));
            var result = _service.GetUnitsAvailable(rentalItem, startDate, nights);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1)]
        public void TestGetCalendar_RentalIdNotExistsOrDateNotInRange(int nights)
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = 1, Units = 3 };
            _repository.Setup(repo => repo.Get(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(new IdentifiedCalendarBookingItem[] { });
            var result = _service.GetCalendar(rentalItem, startDate, nights);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(100, new int[] { 1, 2, 3 }, 3, 3)]
        public void TestGetCalendar_ReturnList(int nights, int[] unitsReserved, int rentalUnits, int reservedNights)
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = 1, Units = rentalUnits };
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(unitsReserved.SelectMany(unit => Enumerable.Range(0, reservedNights + rentalItem.PreparationTimeInDays).Select(day => new IdentifiedCalendarBookingItem()
            {
                RentalId = rentalItem.Id,
                Date = startDate.AddDays(day),
                Unit = unit,
                PreparationAfterBookingId = day >= reservedNights ? 100 : (int?)null,
                Id = $"{startDate.AddDays(day):yyyy-MM-dd}|{rentalItem.Id}|{unit}",
                BookingId = day < reservedNights ? 100 : (int?)null
            })));
            var result = _service.GetCalendar(rentalItem, startDate, nights);
            Assert.Equal((reservedNights + rentalItem.PreparationTimeInDays) * unitsReserved.Count(), result.Count());
        }

        [Fact]
        public void TestCheckUnitsChangeOverlap_RentalIdNotExistsOrUnitNotReserved_ReturnFalse()
        {
            _repository.Setup(repo => repo.Any(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(false);
            Assert.False(_service.CheckUnitsChangeOverlap(1, new int[] { 3 }));
        }

        [Fact]
        public void TestCheckUnitsChangeOverlap_UnitReserved_ReturnTrue()
        {
            _repository.Setup(repo => repo.Any(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(true);
            Assert.True(_service.CheckUnitsChangeOverlap(1, new int[] { 3 }));
        }

        [Theory]
        [InlineData(1, 3, 1, 100, new int[] { 1, 2, 3 })]
        public void TestAddBooking_ReturnAddedKeys(int rentalId, int nights, int preparationDays, int bookingId, int[] units)
        {
            _repository.Setup(repo => repo.InsertRange(It.IsAny<IEnumerable<IdentifiedCalendarBookingItem>>()));
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = rentalId, PreparationTimeInDays = preparationDays, Units = units.Count() };
            var items = _engine.GenerateCalendarBookingItems(
                startDate: startDate,
                rentalId: rentalId,
                nights: nights,
                preparationDays: preparationDays,
                units: units.Take(1).ToArray(),
                bookingId: bookingId);

            var result = _service.AddBooking(rentalItem, startDate, nights, units[0], bookingId);
            Assert.Equal(items.Count(), result.Count());
        }

        [Theory]
        [InlineData(new int[] { 1 }, 3, 1, 3)]
        public void TestExtendPreparationDays_Overlap_ThrowsException(int[] unitsReserved, int rentalUnits, int rentalPreparationDays, int reservedNights)
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = rentalPreparationDays, Units = rentalUnits };
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(
                _engine.GenerateCalendarBookingItems(
                        startDate: startDate,
                        rentalId: rentalItem.Id,
                        nights: reservedNights,
                        preparationDays: rentalPreparationDays,
                        units: unitsReserved,
                        bookingId: 1));
            Assert.Throws<System.Exception>(() => _service.ExtendPreparationDays(rentalItem.Id, 1));
        }

        [Fact]
        public void TestExtendPreparationDays_Extend_ReturnAddedKeys()
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = 1, Units = 3 };
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(new IdentifiedCalendarBookingItem[] {});
            Assert.Empty(_service.ExtendPreparationDays(rentalItem.Id, 1));
        }

        [Fact]
        public void TestShrinkPreparationDays_Shrink_ReturnAddedKeys()
        {
            var startDate = System.DateTime.Today;
            var rentalItem = new VacationRental.Rental.IdentifiedRentalItem() { Id = 1, PreparationTimeInDays = 1, Units = 3 };
            _repository.Setup(repo => repo.Get(It.IsAny<System.Func<IdentifiedCalendarBookingItem, bool>>())).Returns(new IdentifiedCalendarBookingItem[] { });
            Assert.Empty(_service.ShrinkPreparationDays(rentalItem.Id, 1));
        }
        [Fact]
        public void TestCheckRentalHasBookings_RentalIsNotInAnyBookings_ReturnFalse()
        {
            _repository.Setup(repo => repo.Any(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(false);
            Assert.False(_service.CheckRentalHasBookings(1));
        }

        [Fact]
        public void TestCheckRentalHasBookings_HasBookings_ReturnTrue()
        {
            _repository.Setup(repo => repo.Any(It.IsAny<Func<IdentifiedCalendarBookingItem, bool>>())).Returns(true);
            Assert.True(_service.CheckRentalHasBookings(1));
        }
    }
}
