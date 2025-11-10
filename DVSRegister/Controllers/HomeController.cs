using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Models.Home;
using DVSRegister.Models.Register;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("home")]
    public class HomeController(IHomeService homeService, ICabService cabService, ILogger<HomeController> logger) : BaseController(logger)
    {
        private readonly IHomeService homeService = homeService;
        private readonly ICabService cabService = cabService;

        [HttpGet("")]
        [HttpGet("draft-applications")]
        public async Task<ActionResult> DraftApplications(string CurrentSort = "date", string CurrentSortAction = "ascending",int PageNum = 1, string NewSort = "")
        {
            if (NewSort != string.Empty)
            {
                if (CurrentSort == NewSort)
                {
                    CurrentSortAction = CurrentSortAction == "ascending" ? "descending" : "ascending";
                }
                else
                {
                    CurrentSort = NewSort;
                    CurrentSortAction = "ascending";
                }
                PageNum = 1;
            }

            var results = await homeService.GetDraftApplications(CabId, PageNum, CurrentSort, CurrentSortAction);
            var PendingCounts = await homeService.GetPendingCounts(CabId);
            var totalPages = (int)Math.Ceiling((double)results.TotalCount / 10);

            OpenTaskCount openTaskCount = new()
            {
                DraftApplications = PendingCounts["DraftApplications"],
                SentBackApplications = PendingCounts["SentBackApplications"],
                PendingReassignmentRequests = PendingCounts["PendingReassignmentRequests"]
            };

            PendingListViewModel pendingListViewModel = new()
            {
                PendingRequests = results.Items,
                TotalPages = totalPages,
                CurrentSort = CurrentSort,
                CurrentSortAction = CurrentSortAction,
                OpenTaskCount = openTaskCount
            };

            ViewBag.CurrentPage = PageNum;
            return View(pendingListViewModel);
        }

        [HttpGet("sent-back-applications")]
        public async Task<ActionResult> SentBackApplications(string CurrentSort = "date", string CurrentSortAction = "ascending", int PageNum = 1, string NewSort = "")
        {
            if (NewSort != string.Empty)
            {
                if (CurrentSort == NewSort)
                {
                    CurrentSortAction = CurrentSortAction == "ascending" ? "descending" : "ascending";
                }
                else
                {
                    CurrentSort = NewSort;
                    CurrentSortAction = "ascending";
                }
                PageNum = 1;
            }

            var results = await homeService.GetSentBackApplications(CabId, PageNum, CurrentSort, CurrentSortAction);
            var PendingCounts = await homeService.GetPendingCounts(CabId);
            var totalPages = (int)Math.Ceiling((double)results.TotalCount / 10);

            OpenTaskCount openTaskCount = new()
            {
                DraftApplications = PendingCounts["DraftApplications"],
                SentBackApplications = PendingCounts["SentBackApplications"],
                PendingReassignmentRequests = PendingCounts["PendingReassignmentRequests"]
            };

            PendingListViewModel pendingListViewModel = new()
            {
                PendingRequests = results.Items,
                TotalPages = totalPages,
                CurrentSort = CurrentSort,
                CurrentSortAction = CurrentSortAction,
                OpenTaskCount = openTaskCount
            };

            ViewBag.CurrentPage = PageNum;
            return View(pendingListViewModel);
        }

        [HttpGet("pending-reassignment-requests")]
        public async Task<ActionResult> PendingReassignmentRequests(string CurrentSort = "dateAfterPublish", string CurrentSortAction = "ascending", int PageNum = 1, string NewSort = "")
        {
            if (NewSort != string.Empty)
            {
                if (CurrentSort == NewSort)
                {
                    CurrentSortAction = CurrentSortAction == "ascending" ? "descending" : "ascending";
                }
                else
                {
                    CurrentSort = NewSort;
                    CurrentSortAction = "ascending";
                }
                PageNum = 1;
            }

            var results = await homeService.GetPendingReassignmentRequests(CabId, PageNum, CurrentSort, CurrentSortAction);
            var PendingCounts = await homeService.GetPendingCounts(CabId);
            var totalPages = (int)Math.Ceiling((double)results.TotalCount / 10);

            OpenTaskCount openTaskCount = new()
            {
                DraftApplications = PendingCounts["DraftApplications"],
                SentBackApplications = PendingCounts["SentBackApplications"],
                PendingReassignmentRequests = PendingCounts["PendingReassignmentRequests"]
            };

            PendingListViewModel pendingListViewModel = new()
            {
                PendingRequests = results.Items,
                TotalPages = totalPages,
                CurrentSort = CurrentSort,
                CurrentSortAction = CurrentSortAction,
                OpenTaskCount = openTaskCount
            };

            ViewBag.CurrentPage = PageNum;
            return View(pendingListViewModel);
        }

        [HttpGet("all-providers")]
        public async Task<IActionResult> AllProviders(string CurrentSort = "date", string CurrentSortAction = "descending", string NewSort = "", string SearchText = "",
            string searchAction = "", int PageNum = 1)
        {
            if (!string.IsNullOrEmpty(NewSort))
            {
                if (CurrentSort == NewSort)
                {
                    CurrentSortAction = CurrentSortAction == "ascending" ? "descending" : "ascending";
                }
                else
                {
                    CurrentSort = NewSort;
                    CurrentSortAction = "ascending";
                }
                PageNum = 1;
            }

            if (searchAction == "clearSearch")
            {
                ModelState.Clear();
                SearchText = string.Empty;
            }

            if (PageNum < 1)
            {
                PageNum = 1;
            }

            var results = await homeService.GetAllProviders(CabId, PageNum, CurrentSort, CurrentSortAction, SearchText);
            var totalPages = (int)Math.Ceiling((double)results.TotalCount / 10);

            var vm = new AllProvidersViewModel
            {
                Providers = results.Items,
                CurrentSort = CurrentSort,
                CurrentSortAction = CurrentSortAction,
                SearchText = SearchText,
                TotalPages = totalPages
            };

            ViewBag.CurrentPage = PageNum;

            return View(vm);
        }

        [HttpGet("provider-details/{providerId}")]
        public async Task<IActionResult> ProviderDetails(int providerId)
        {
            var provider = await cabService.GetProvider(providerId, CabId);
            
            if (provider == null)
            {
                return NotFound();
            }

            ViewBag.IsCompanyInfoEditable = cabService.CheckCompanyInfoEditable(provider);
            ViewBag.IsEditable = true;

            return View(provider);
        }
    }
}




