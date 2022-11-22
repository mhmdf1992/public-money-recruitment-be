using BookingRentals.Rental;
using Moq;
using Repository;
using VacationRental.Rental;
using Xunit;

namespace VacationRantal.Tests
{
    public class RentalServiceTests
    {
        IRental _service;
        IRentalEngine _engine;
        Mock<IRepository<IdentifiedRentalItem>> _repository;
        public RentalServiceTests()
        {
            _repository =  new Mock<IRepository<IdentifiedRentalItem>>();
            _service = new RentalService(_repository.Object);
            _engine = _service as IRentalEngine;
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
        public void TestGetRentalById_RentalIdExists_ReturnRentalItem(int id, int preparationTimeInDays, int units)
        {
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Returns(new IdentifiedRentalItem() { Id = id, PreparationTimeInDays = preparationTimeInDays, Units = units});
            var rentalItem = _service.GetRentalById(id);
            Assert.Equal(id, rentalItem.Id);
            Assert.Equal(preparationTimeInDays, rentalItem.PreparationTimeInDays);
            Assert.Equal(units, rentalItem.Units);
        }

        [Theory]
        [InlineData(1)]
        public void TestGetRentalById_RentalIdNotExists_ThrowsResourceNotFountException(int id)
        {
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Throws(new Repository.Exceptions.ResourceNotFoundException(It.IsAny<string>()));
            Assert.Throws<Repository.Exceptions.ResourceNotFoundException>(() => _service.GetRentalById(id));
        }

        [Theory]
        [InlineData( -1, -1)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        public void TestAddRental_InvalidArguments_ThrowsArgumentException(int preparationTimeInDays, int units)
        {
            Assert.Throws<System.ArgumentException>(() => _service.AddRental(units, preparationTimeInDays));
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(2, 0, 2)]
        [InlineData(3, 1, 3)]
        public void TestAddRental_GetRental_ReturnAddedRental(int mockedId, int preparationTimeInDays, int units)
        {
            _repository.Setup(repo => repo.Insert(It.IsAny<IdentifiedRentalItem>())).Returns(mockedId.ToString());
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Returns(new IdentifiedRentalItem() { Id = mockedId, PreparationTimeInDays = preparationTimeInDays, Units = units });
            var id = _service.AddRental(units, preparationTimeInDays);
            var rentalItem = _service.GetRentalById(id);
            Assert.Equal(mockedId, rentalItem.Id);
            Assert.Equal(preparationTimeInDays, rentalItem.PreparationTimeInDays);
            Assert.Equal(units, rentalItem.Units);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        public void TestUpdateRental_InvalidArguments_ThrowsArgumentException(int preparationTimeInDays, int units)
        {
            Assert.Throws<System.ArgumentException>(() => _service.AddRental(units, preparationTimeInDays));
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(2, 0, 2)]
        [InlineData(3, 1, 3)]
        public void TestUpdateRental_GetRental_ReturnUpdatedRental(int mockedId, int preparationTimeInDays, int units)
        {
            System.Action<IdentifiedRentalItem> doNothing = x => { };
            _repository.Setup(repo => repo.Update(It.IsAny<string>(), doNothing) ).Returns(mockedId.ToString());
            _repository.Setup(repo => repo.Get(It.IsAny<string>())).Returns(new IdentifiedRentalItem() { Id = mockedId, PreparationTimeInDays = preparationTimeInDays + 1, Units = units + 1 });
            var id = _service.UpdateRental(mockedId, units + 1, preparationTimeInDays + 1);
            var rentalItem = _service.GetRentalById(id);
            Assert.Equal(mockedId, rentalItem.Id);
            Assert.Equal(preparationTimeInDays + 1, rentalItem.PreparationTimeInDays);
            Assert.Equal(units + 1, rentalItem.Units);
        }
    }
}

