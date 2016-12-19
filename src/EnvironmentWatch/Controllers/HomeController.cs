using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EnvironmentWatch.Models;

namespace EnvironmentWatch.Controllers
{
    public class HomeController : Controller
    {
        #region Members

        private readonly EnvWatchContext _context;

        #endregion Members

        #region Constructor

        /// <summary>
        /// Constructor with EF context fed in by containter/injection
        /// </summary>
        /// <param name="context"></param>
        public HomeController(EnvWatchContext context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Actions

        /// <summary>
        /// Home page - show device list, locations, last 10 
        /// measurements for a given device, and graph if possible.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Index(int? id = null)
        {
            var vm = new DeviceInfo { ReportingDeviceId = id };
            ReportingDevice device = null;

            // set lists of devices and locations for display
            if (_context.ReportingDevices.Any())
            {
                vm.Devices = _context.ReportingDevices.ToList();

                // get first device here since we know the list has data
                device = vm.Devices.First();
            }

            if (_context.Locations.Any())
            {
                vm.Locations = _context.Locations.ToList();
            }

            // load given device if possible
            if (id.HasValue)
            {
                device = _context.ReportingDevices.FirstOrDefault(d => d.ReportingDeviceId == id.Value);
            }

            if (device != null)
            {
                vm.ReportingDeviceId = device.ReportingDeviceId;
                vm.TypeName = device.Name;
                vm.LocationName = device.Location.Name;
                vm.LocalIp = device.LastIpAddress;
                vm.LastSet = MostRecentSet(device.ReportingDeviceId);
            }

            return View(vm);
        }


        /// <summary>
        /// Return data for a day/24 hours for given device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeviceDay(int? id)
        {
            // establish an empty table
            var gdataTable = new GoogleVizDataTable();

            gdataTable.cols.Add(new GoogleVizDataTable.Col { label = "Time of Day", type = "datetime" });
            gdataTable.cols.Add(new GoogleVizDataTable.Col { label = "Temp F", type = "number" });
            gdataTable.cols.Add(new GoogleVizDataTable.Col { label = "Humidity %", type = "number" });
            gdataTable.cols.Add(new GoogleVizDataTable.Col { label = "Light %", type = "number" });

            // if ID given is present
            if (id.HasValue)
            {
                // next get the most recent measurement for this device
                var mostRecent = _context.Measurements.Where(d => d.ReportingDeviceId == id.Value)
                    .Select(m => m).OrderByDescending(m => m.MeasuredDate).Take(1).FirstOrDefault();

                // if we have a recent measurement for this device
                if (mostRecent != null)
                {
                    // establish a range of previous to current day/time
                    var finish = mostRecent.MeasuredDate;
                    var start = finish.AddDays(-1);

                    // fetch a set of measurements for that range
                    var recentSet = MeasureSetRange(id.Value, start, finish);

                    // build out the google datatable using this data
                    gdataTable.rows =
                        (from set in recentSet
                         select new GoogleVizDataTable.Row
                         {
                            c = new List<GoogleVizDataTable.Row.RowValue>
                            {
                                new GoogleVizDataTable.Row.RowValue { v = set.GoogleDate },
                                new GoogleVizDataTable.Row.RowValue { v = set.TempString },
                                new GoogleVizDataTable.Row.RowValue { v = set.HumidString },
                                new GoogleVizDataTable.Row.RowValue { v = set.LightString }
                            }
                         }).ToList();
                }

            }

            return Json(gdataTable);
        }


        /// <summary>
        /// Description of the site, purpose and electronics
        /// Includes a board layout image
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            return View();
        }


        /// <summary>
        /// Intertech contact info
        /// </summary>
        /// <returns></returns>
        public IActionResult Contact()
        {
            return View();
        }


        /// <summary>
        /// Default error page
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            return View();
        }


        /// <summary>
        /// Show view allowing add of a location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddLocation()
        {
            return View(new LocationHandler());
        }


        /// <summary>
        /// Do the work of adding a location
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddLocation(LocationHandler model)
        {
            if (@ModelState.IsValid)
            {
                // check if name is in use
                if (_context.Locations.Any(l => l.Name.Equals(model.LocationName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    model.Success = false;
                    model.Message = "Name is in use";
                }
                else
                {
                    // didn't use an identity seed for location so I have to manually increment
                    var addedId = _context.Locations.Max(l => l.LocationId) + 1;
                    var addedLoc = new Location
                    { LocationId = addedId, Name = model.LocationName, Description = model.LocationDesc };

                    _context.Locations.Add(addedLoc);
                    _context.SaveChanges();

                    // will not assume user wants to move a device to this location yet so just head back to home page
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }


        /// <summary>
        /// Move a device into a new location
        /// </summary>
        /// <param name="reportingDeviceId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public ActionResult ChangeLocation(int reportingDeviceId, int locationId)
        {
            var model = new LocationHandler { ReportingDeviceId = reportingDeviceId, Success = false };
            var device = _context.ReportingDevices.Include(t => t.DeviceType).FirstOrDefault(d => d.ReportingDeviceId == reportingDeviceId);
            var location = _context.Locations.FirstOrDefault(l => l.LocationId == locationId);

            if (device == null) { model.Message = $"Device with ID {reportingDeviceId} not found"; }
            else if (location == null) { model.Message = $"Location with ID {locationId} not found"; }
            else
            {
                device.LocationId = location.LocationId;
                _context.ReportingDevices.Attach(device);
                _context.Entry(device).State = EntityState.Modified;
                _context.SaveChanges();

                model.DeviceTypeName = device.DeviceType.Name;
                model.LocationName = location.Name;
                model.Success = true;
            }

            return View(model);
        }


        /// <summary>
        /// Method that receives the three values from the device and posts them to the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ip"></param>
        /// <param name="temp"></param>
        /// <param name="humidity"></param>
        /// <param name="light"></param>
        /// <returns></returns>
        public ActionResult PostData(int id, string ip, decimal? temp, decimal? humidity, decimal? light)
        {
            var results = "Success";
            var reported = DateTime.Now;

            try
            {
                var device = _context.ReportingDevices.FirstOrDefault(d => d.ReportingDeviceId == id);

                if (device == null)
                {
                    results = "Unknown device";
                }
                else
                {
                    // update the ip address first
                    device.LastIpAddress = ip;

                    if (temp.HasValue)
                    {
                        // add temperature
                        _context.Measurements.Add(new Measurement
                        {
                            MeasurementTypeId = (int)MeasureTypeEnum.Temperature,
                            ReportingDeviceId = device.ReportingDeviceId,
                            LocationId = device.LocationId,
                            MeasuredValue = temp.Value,
                            MeasuredDate = reported
                        });
                    }

                    if (humidity.HasValue)
                    {
                        // add humidity
                        _context.Measurements.Add(new Measurement
                        {
                            MeasurementTypeId = (int)MeasureTypeEnum.Humidity,
                            ReportingDeviceId = device.ReportingDeviceId,
                            LocationId = device.LocationId,
                            MeasuredValue = humidity.Value,
                            MeasuredDate = reported
                        });
                    }

                    if (light.HasValue)
                    {
                        // add light
                        _context.Measurements.Add(new Measurement
                        {
                            MeasurementTypeId = (int)MeasureTypeEnum.Light,
                            ReportingDeviceId = device.ReportingDeviceId,
                            LocationId = device.LocationId,
                            MeasuredValue = light.Value,
                            MeasuredDate = reported
                        });
                    }

                    // save it all
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                results = "Exception: " + ex.Message;
            }

            return Content(results);
        }

        #endregion Actions

        #region Supporting

        /// <summary>
        /// Get the most recent set of measurements for a specific device
        /// </summary>
        /// <param name="reportingDeviceId"></param>
        /// <returns></returns>
        public MeasurementSet MostRecentSet(int reportingDeviceId)
        {
            var recent = new MeasurementSet();

            var last3 = _context.Measurements
                .Where(m => m.ReportingDeviceId == reportingDeviceId)
                .Select(m => m).Include(l => l.Location).Distinct().
                OrderByDescending(m => m.MeasuredDate).Take(3).ToList();

            if (last3.Any())
            {
                var temp = last3.FirstOrDefault(m => m.MeasurementTypeId == 1);
                var humd = last3.FirstOrDefault(m => m.MeasurementTypeId == 2);
                var lght = last3.FirstOrDefault(m => m.MeasurementTypeId == 3);

                if (temp != null)
                {
                    recent.MeasuredDate = temp.MeasuredDate;
                    recent.Temperature = temp.MeasuredValue;
                }

                if (humd != null) { recent.Humidity = humd.MeasuredValue; }
                if (lght != null) { recent.Light = lght.MeasuredValue; }
            }

            return recent;
        }


        /// <summary>
        /// Build an aggregate list last day's worth of measurements, i.e.
        /// from the most recent measurement back to 24 hours previous, but
        /// averaged by hour
        /// </summary>
        /// <param name="reportingDeviceId">Specific device ID for which to fetch a set of measurments</param>
        /// <param name="start">Start date/time for which to fetch set of measurements</param>
        /// <param name="finish">Finishing date/time for which to fetch set of measurements</param>
        /// <returns></returns>
        public List<MeasurementSet> MeasureSetRange(int reportingDeviceId, DateTime start, DateTime finish)
        {
            // build the list of measure sets
            var measureSet =
                (from m in _context.Measurements
                    where m.ReportingDeviceId == reportingDeviceId
                    && m.MeasuredDate >= start
                    && m.MeasuredDate <= finish
                    orderby m.MeasuredDate
                    group m by new { MeasuredDate = DateTime.Parse(m.MeasuredDate.ToString("yyyy-MM-dd HH:mm:ss")), m.Location.Name }
                    into g
                    select new MeasurementSet
                    {
                        MeasuredDate = g.Key.MeasuredDate,
                        LocationName = g.Key.Name,
                        Temperature = g.Where(m => m.MeasurementTypeId == 1).Select(r => r.MeasuredValue).FirstOrDefault(),
                        Humidity = g.Where(m => m.MeasurementTypeId == 2).Select(r => r.MeasuredValue).FirstOrDefault(),
                        Light = g.Where(m => m.MeasurementTypeId == 3).Select(r => r.MeasuredValue).FirstOrDefault()
                    }).ToList();

            return measureSet;
        }

        #endregion Supporting
    }
}
