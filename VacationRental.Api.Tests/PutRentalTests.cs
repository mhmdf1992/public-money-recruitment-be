using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PutRentalTests
    {
        private readonly HttpClient _client;

        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task TestPutRental_EmptyBookings_ThenAGetReturnsTheEditedRental()
        {
            var postRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 25
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/vacationrental/rentals", postRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 1,
                Units = 1
            };

            ResourceIdViewModel putResult;
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                putResult = await putResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/vacationrental/rentals/{putResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<CleanedRentalViewModel>();
                Assert.Equal(putRequest.Units, getResult.Units);
                Assert.Equal(putRequest.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task TestPutRental_OverlapInPreparationDays_ThrowsApplicationException()
        {
            var postRentalRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 1,
                Units = 3
            };
            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/vacationrental/rentals", postRentalRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now
            };
            ResourceIdViewModel postBookingResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest2 = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now.AddDays(5)
            };

            ResourceIdViewModel postBookingResult2;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest2))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult2 = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 3,
                Units = 3
            };

            await Assert.ThrowsAsync<System.ApplicationException>(() => _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRequest));
        }

        [Fact]
        public async Task TestPutRental_OverlapInUnits_ThrowsApplicationException()
        {
            var postRentalRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 1,
                Units = 3
            };
            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/vacationrental/rentals", postRentalRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now
            };
            ResourceIdViewModel postBookingResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest2 = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now
            };

            ResourceIdViewModel postBookingResult2;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest2))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult2 = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 3,
                Units = 1
            };

            await Assert.ThrowsAsync<System.ApplicationException>(() => _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRequest));
        }

        [Fact]
        public async Task TestPutRental_PreparationDaysExtend_Ok()
        {
            var postRentalRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 1,
                Units = 3
            };
            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/vacationrental/rentals", postRentalRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now
            };
            ResourceIdViewModel postBookingResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest2 = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now.AddDays(5)
            };

            ResourceIdViewModel postBookingResult2;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest2))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult2 = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 2,
                Units = 3
            };

            ResourceIdViewModel putResult;
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                putResult = await putResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start={DateTime.Now:yyyy-MM-dd}&nights=5"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();

                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Empty(getCalendarResult.Dates[3].Bookings);
                Assert.Empty(getCalendarResult.Dates[4].Bookings);
            }
        }
    

        [Fact]
        public async Task TestPutRental_PreparationDaysShrink_Ok()
        {
            var postRentalRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 2,
                Units = 3
            };
            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/vacationrental/rentals", postRentalRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest = new BookingBindingModel()
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = DateTime.Now
            };
            ResourceIdViewModel postBookingResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRequest = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 1,
                Units = 3
            };

            ResourceIdViewModel putResult;
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                putResult = await putResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start={DateTime.Now:yyyy-MM-dd}&nights=5"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();

                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(4, getCalendarResult.Dates.Count());
                Assert.Empty(getCalendarResult.Dates[3].Bookings);
            }
        }
    }
}
