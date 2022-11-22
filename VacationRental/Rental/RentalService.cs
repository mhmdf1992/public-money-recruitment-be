using BookingRentals.Calendar;
using BookingRentals.Rental;
using System;
using System.Linq;

namespace VacationRental.Rental
{
    public class RentalService : IRental, IRentalEngine
    {
        protected readonly Repository.IRepository<IdentifiedRentalItem> _repository;
        public RentalService(Repository.IRepository<IdentifiedRentalItem> repository)
        {
            _repository = repository;
        }

        public int AddRental(int units, int preparationTimeInDays = 0)
        {
            if (units <= 0)
                throw new ArgumentException($"{units} is not valid {nameof(units)}", nameof(units));
            if (preparationTimeInDays < 0)
                throw new ArgumentException($"{preparationTimeInDays} is not valid {nameof(preparationTimeInDays)}", nameof(preparationTimeInDays));
            try
            {

                var id = _repository.Insert(new IdentifiedRentalItem()
                {
                    Id = GenerateId(),
                    PreparationTimeInDays = preparationTimeInDays,
                    Units = units
                });
                return int.Parse(id);
            }
            catch { throw; }
        }

        public int GenerateId() => (int)_repository.Count() + 1;

        public RentalItem GetRentalById(int id)
        {
            try
            {
                return _repository.Get(id.ToString());
            }
            catch { throw; }
        }

        public int UpdateRental(int id, int units, int preparationTimeInDays)
        {
            if (units <= 0)
                throw new ArgumentException($"{units} is not valid {nameof(units)}", nameof(units));
            if (preparationTimeInDays < 0)
                throw new ArgumentException($"{preparationTimeInDays} is not valid {nameof(preparationTimeInDays)}", nameof(preparationTimeInDays));
            try
            {
                int.TryParse(_repository.Update(id.ToString(), x => {
                    x.Units = units;
                    x.PreparationTimeInDays = preparationTimeInDays;
                }), out int retId);
                return retId;
            }
            catch { throw; }
        }
    }
}
