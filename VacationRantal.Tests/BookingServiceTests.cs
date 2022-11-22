using BookingRentals.Booking;
using Moq;
using Repository;
using VacationRental.Booking;
using Xunit;

namespace VacationRantal.Tests
{
    public class BookingServiceTests
    {
        IBooking _service;
        IBookingEngine _engine;
        Mock<IRepository<IdentifiedBookingItem>> _repository;
        public BookingServiceTests()
        {
            _repository =  new Mock<IRepository<IdentifiedBookingItem>>();
            _service = new BookingService(_repository.Object);
            _engine = _service as IBookingEngine;
        }

        [Theory]
        [InlineData(10)]
        public void TestGenerateId_ReturnRepositoryCountPlus1(int count)
        {
            _repository.Setup(repo => repo.Count()).Returns(count);
            Assert.Equal(count + 1, _engine.GenerateId());
        }

        [Theory]
        [InlineData(1, 1, 3)]
        public void TestGetBookingById_BookingIdExists_ReturnbookingItem(int id, int rentalId, int nights)
        {
            var start = System.DateTime.Now;
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Returns(new IdentifiedBookingItem() { Id = id, RentalId = rentalId, Nights = nights, Start = start});
            var bookingItem = _service.GetBookingById(id);
            Assert.Equal(id, bookingItem.Id);
            Assert.Equal(rentalId, bookingItem.RentalId);
            Assert.Equal(nights, bookingItem.Nights);
            Assert.Equal(start, bookingItem.Start);
        }

        [Theory]
        [InlineData(1)]
        public void TestGetBookingById_IdNotExists_ThrowsResourceNotFountException(int id)
        {
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Throws(new Repository.Exceptions.ResourceNotFoundException(It.IsAny<string>()));
            Assert.Throws<Repository.Exceptions.ResourceNotFoundException>(() => _service.GetBookingById(id));
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(1, 0)]
        public void TestAddBooking_InvalidArguments_ThrowsArgumentException(int rentalId, int nights)
        {
            var start = System.DateTime.Today;
            Assert.Throws<System.ArgumentException>(() => _service.AddBooking(rentalId, start, nights));
        }

        [Theory]
        [InlineData(1, 1, 3)]
        [InlineData(2, 1, 4)]
        [InlineData(3, 1, 5)]
        public void TestAddBooking_GetBooking_ReturnAddedBooking(int mockedId, int rentalId, int nights)
        {
            var start = System.DateTime.Today;
            _repository.Setup(repo => repo.Insert(It.IsAny<IdentifiedBookingItem>())).Returns(mockedId.ToString());
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Returns(new IdentifiedBookingItem() { Id = mockedId, RentalId = rentalId, Start = start, Nights = nights });
            var id = _service.AddBooking(rentalId, start, nights);
            var bookingItem = _service.GetBookingById(id);
            Assert.Equal(mockedId, bookingItem.Id);
            Assert.Equal(rentalId, bookingItem.RentalId);
            Assert.Equal(nights, bookingItem.Nights);
            Assert.Equal(start, bookingItem.Start);
        }
    }
}

