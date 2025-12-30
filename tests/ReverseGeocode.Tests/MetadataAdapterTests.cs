using System.Text.Json;
using ReverseGeocode.Google;
using ReverseGeocode.Models;

namespace ReverseGeocode.Tests;

public class MetadataConverterTests
{
    [Fact]
    public void TestConvertingResponse()
    {
        var adapter = new MetadataAdapter();
        var svc = new GoogleMapService("api-key not needed");
        var location = new Models.Location(Guid.CreateVersion7(), 51.501100m, -0.121800m);
        var response = JsonSerializer.Deserialize<ReverseGeocodeResponse>(EXAMPLE);
        var result = svc.BuildResult(response);
        var metadata = adapter.ConvertGoogleReponse(location, result);

        Assert.Equal("Westminster Pier, London SW1A 2JH, UK", result.FormattedAddress);
        Assert.Equal("England", metadata.AdministrativeAreaLevel1);
        Assert.Equal("Greater London", metadata.AdministrativeAreaLevel2);
        Assert.Null(metadata.AdministrativeAreaLevel3);
        Assert.Equal("United Kingdom", metadata.Country);
        Assert.Equal("London", metadata.Locality);
        Assert.Null(metadata.Neighborhood);
        Assert.Equal("SW1A 2JH", metadata.PostalCode);
        Assert.Null(metadata.PostalCodeSuffix);
        Assert.Equal("Westminster Pier", metadata.Premise);
        Assert.Equal("Westminster Bridge", metadata.Route);
        Assert.Null(metadata.StreetNumber);
        Assert.Null(metadata.SubLocalityLevel1);
        Assert.Null(metadata.SubLocalityLevel2);
        Assert.Null(metadata.SubPremise);

        Assert.Single(metadata.PointsOfInterest);
        Assert.Equal("Transit Station", metadata.PointsOfInterest.First().Type);
        Assert.Equal("Westminster Pier", metadata.PointsOfInterest.First().Name);
    }

    // https://maps.googleapis.com/maps/api/geocode/json?latlng=51.501100,-0.121800&key=<apikey>
    const string EXAMPLE =
        """
        {
            "plus_code": {
                "compound_code": "GV2H+C7W London, UK",
                "global_code": "9C3XGV2H+C7W"
            },
            "results": [
                {
                    "address_components": [
                        {
                            "long_name": "Westminster Pier",
                            "short_name": "Westminster Pier",
                            "types": [
                                "premise"
                            ]
                        },
                        {
                            "long_name": "London",
                            "short_name": "London",
                            "types": [
                                "postal_town"
                            ]
                        },
                        {
                            "long_name": "Greater London",
                            "short_name": "Greater London",
                            "types": [
                                "administrative_area_level_2",
                                "political"
                            ]
                        },
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        },
                        {
                            "long_name": "SW1A 2JH",
                            "short_name": "SW1A 2JH",
                            "types": [
                                "postal_code"
                            ]
                        }
                    ],
                    "formatted_address": "Westminster Pier, London SW1A 2JH, UK",
                    "geometry": {
                        "location": {
                            "lat": 51.5016904,
                            "lng": -0.1232218
                        },
                        "location_type": "ROOFTOP",
                        "viewport": {
                            "northeast": {
                                "lat": 51.5030393802915,
                                "lng": -0.121872819708498
                            },
                            "southwest": {
                                "lat": 51.5003414197085,
                                "lng": -0.124570780291502
                            }
                        }
                    },
                    "navigation_points": [
                        {
                            "location": {
                                "latitude": 51.5016939,
                                "longitude": -0.1233262
                            },
                            "restricted_travel_modes": [
                                "DRIVE"
                            ]
                        },
                        {
                            "location": {
                                "latitude": 51.500993,
                                "longitude": -0.1236585
                            },
                            "restricted_travel_modes": [
                                "WALK"
                            ]
                        }
                    ],
                    "place_id": "ChIJ4TpODTsDdkgRgnUQZBvdWkc",
                    "plus_code": {
                        "compound_code": "GV2G+MP London, UK",
                        "global_code": "9C3XGV2G+MP"
                    },
                    "types": [
                        "establishment",
                        "point_of_interest",
                        "tourist_attraction",
                        "travel_agency"
                    ]
                },
                {
                    "address_components": [
                        {
                            "long_name": "Westminster Pier",
                            "short_name": "Westminster Pier",
                            "types": [
                                "establishment",
                                "point_of_interest",
                                "transit_station"
                            ]
                        },
                        {
                            "long_name": "London",
                            "short_name": "London",
                            "types": [
                                "postal_town"
                            ]
                        },
                        {
                            "long_name": "Greater London",
                            "short_name": "Greater London",
                            "types": [
                                "administrative_area_level_2",
                                "political"
                            ]
                        },
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        },
                        {
                            "long_name": "SW1A 2JH",
                            "short_name": "SW1A 2JH",
                            "types": [
                                "postal_code"
                            ]
                        }
                    ],
                    "formatted_address": "Westminster Pier, London SW1A 2JH, UK",
                    "geometry": {
                        "location": {
                            "lat": 51.50156819999999,
                            "lng": -0.123603
                        },
                        "location_type": "GEOMETRIC_CENTER",
                        "viewport": {
                            "northeast": {
                                "lat": 51.5029171802915,
                                "lng": -0.122254019708498
                            },
                            "southwest": {
                                "lat": 51.5002192197085,
                                "lng": -0.124951980291502
                            }
                        }
                    },
                    "navigation_points": [
                        {
                            "location": {
                                "latitude": 51.5015721,
                                "longitude": -0.1236331
                            },
                            "restricted_travel_modes": [
                                "DRIVE"
                            ]
                        },
                        {
                            "location": {
                                "latitude": 51.5015663,
                                "longitude": -0.1239712
                            },
                            "restricted_travel_modes": [
                                "WALK"
                            ]
                        }
                    ],
                    "place_id": "ChIJBXhU1cUEdkgRAgyEb4K8fXY",
                    "plus_code": {
                        "compound_code": "GV2G+JH London, UK",
                        "global_code": "9C3XGV2G+JH"
                    },
                    "types": [
                        "establishment",
                        "point_of_interest",
                        "transit_station"
                    ]
                },
                {
                    "address_components": [
                        {
                            "long_name": "GV2H+C7",
                            "short_name": "GV2H+C7",
                            "types": [
                                "plus_code"
                            ]
                        },
                        {
                            "long_name": "London",
                            "short_name": "London",
                            "types": [
                                "postal_town"
                            ]
                        },
                        {
                            "long_name": "Greater London",
                            "short_name": "Greater London",
                            "types": [
                                "administrative_area_level_2",
                                "political"
                            ]
                        },
                        {
                            "long_name": "England",
                            "short_name": "England",
                            "types": [
                                "administrative_area_level_1",
                                "political"
                            ]
                        },
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        }
                    ],
                    "formatted_address": "GV2H+C7 London, UK",
                    "geometry": {
                        "bounds": {
                            "northeast": {
                                "lat": 51.50112499999999,
                                "lng": -0.12175
                            },
                            "southwest": {
                                "lat": 51.501,
                                "lng": -0.121875
                            }
                        },
                        "location": {
                            "lat": 51.5011,
                            "lng": -0.1218
                        },
                        "location_type": "GEOMETRIC_CENTER",
                        "viewport": {
                            "northeast": {
                                "lat": 51.5024114802915,
                                "lng": -0.120463519708498
                            },
                            "southwest": {
                                "lat": 51.4997135197085,
                                "lng": -0.123161480291502
                            }
                        }
                    },
                    "place_id": "GhIJRwN4CyTASUAR5R2n6Eguv78",
                    "plus_code": {
                        "compound_code": "GV2H+C7 London, UK",
                        "global_code": "9C3XGV2H+C7"
                    },
                    "types": [
                        "plus_code"
                    ]
                },
                {
                    "address_components": [
                        {
                            "long_name": "Westminster Bridge",
                            "short_name": "A302",
                            "types": [
                                "route"
                            ]
                        },
                        {
                            "long_name": "London",
                            "short_name": "London",
                            "types": [
                                "postal_town"
                            ]
                        },
                        {
                            "long_name": "Greater London",
                            "short_name": "Greater London",
                            "types": [
                                "administrative_area_level_2",
                                "political"
                            ]
                        },
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        }
                    ],
                    "formatted_address": "A302, London, UK",
                    "geometry": {
                        "bounds": {
                            "northeast": {
                                "lat": 51.5008638,
                                "lng": -0.1199522
                            },
                            "southwest": {
                                "lat": 51.5008011,
                                "lng": -0.1220051
                            }
                        },
                        "location": {
                            "lat": 51.5008305,
                            "lng": -0.1209788
                        },
                        "location_type": "GEOMETRIC_CENTER",
                        "viewport": {
                            "northeast": {
                                "lat": 51.5021814302915,
                                "lng": -0.119629669708498
                            },
                            "southwest": {
                                "lat": 51.4994834697085,
                                "lng": -0.122327630291502
                            }
                        }
                    },
                    "place_id": "ChIJOZc178YEdkgRmL-OMmKOugI",
                    "types": [
                        "route"
                    ]
                },
                {
                    "address_components": [
                        {
                            "long_name": "London",
                            "short_name": "London",
                            "types": [
                                "locality",
                                "political"
                            ]
                        },
                        {
                            "long_name": "London",
                            "short_name": "London",
                            "types": [
                                "postal_town"
                            ]
                        },
                        {
                            "long_name": "Greater London",
                            "short_name": "Greater London",
                            "types": [
                                "administrative_area_level_2",
                                "political"
                            ]
                        },
                        {
                            "long_name": "England",
                            "short_name": "England",
                            "types": [
                                "administrative_area_level_1",
                                "political"
                            ]
                        },
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        }
                    ],
                    "formatted_address": "London, UK",
                    "geometry": {
                        "bounds": {
                            "northeast": {
                                "lat": 51.6723432,
                                "lng": 0.148271
                            },
                            "southwest": {
                                "lat": 51.38494009999999,
                                "lng": -0.3514683
                            }
                        },
                        "location": {
                            "lat": 51.5072178,
                            "lng": -0.1275862
                        },
                        "location_type": "APPROXIMATE",
                        "viewport": {
                            "northeast": {
                                "lat": 51.6723432,
                                "lng": 0.148271
                            },
                            "southwest": {
                                "lat": 51.38494009999999,
                                "lng": -0.3514683
                            }
                        }
                    },
                    "place_id": "ChIJdd4hrwug2EcRmSrV3Vo6llI",
                    "types": [
                        "locality",
                        "political"
                    ]
                },
                {
                    "address_components": [
                        {
                            "long_name": "Greater London",
                            "short_name": "Greater London",
                            "types": [
                                "administrative_area_level_2",
                                "political"
                            ]
                        },
                        {
                            "long_name": "England",
                            "short_name": "England",
                            "types": [
                                "administrative_area_level_1",
                                "political"
                            ]
                        },
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        }
                    ],
                    "formatted_address": "Greater London, UK",
                    "geometry": {
                        "bounds": {
                            "northeast": {
                                "lat": 51.6918728,
                                "lng": 0.3340155
                            },
                            "southwest": {
                                "lat": 51.2867918,
                                "lng": -0.5103259999999999
                            }
                        },
                        "location": {
                            "lat": 51.5569879,
                            "lng": -0.1411791
                        },
                        "location_type": "APPROXIMATE",
                        "viewport": {
                            "northeast": {
                                "lat": 51.6918728,
                                "lng": 0.3340155
                            },
                            "southwest": {
                                "lat": 51.2867918,
                                "lng": -0.5103259999999999
                            }
                        }
                    },
                    "place_id": "ChIJb-IaoQug2EcRi-m4hONz8S8",
                    "types": [
                        "administrative_area_level_2",
                        "political"
                    ]
                },
                {
                    "address_components": [
                        {
                            "long_name": "United Kingdom",
                            "short_name": "GB",
                            "types": [
                                "country",
                                "political"
                            ]
                        }
                    ],
                    "formatted_address": "United Kingdom",
                    "geometry": {
                        "bounds": {
                            "northeast": {
                                "lat": 60.91569999999999,
                                "lng": 33.9165549
                            },
                            "southwest": {
                                "lat": 34.5614,
                                "lng": -8.8988999
                            }
                        },
                        "location": {
                            "lat": 55.378051,
                            "lng": -3.435973
                        },
                        "location_type": "APPROXIMATE",
                        "viewport": {
                            "northeast": {
                                "lat": 60.91569999999999,
                                "lng": 33.9165549
                            },
                            "southwest": {
                                "lat": 34.5614,
                                "lng": -8.8988999
                            }
                        }
                    },
                    "place_id": "ChIJqZHHQhE7WgIReiWIMkOg-MQ",
                    "types": [
                        "country",
                        "political"
                    ]
                }
            ],
            "status": "OK"
        }
        """;
}
