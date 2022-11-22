using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PostVacationRentRentalTests
    {
        private readonly HttpClient _client;

        public PostVacationRentRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new CleanedRentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 25
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/vacationrental/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                Assert.True(postResult.Id > 0);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/vacationrental/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<CleanedRentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
                Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }
    }
}
