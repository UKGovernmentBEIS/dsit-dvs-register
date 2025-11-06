using DVSRegister.BusinessLogic.Services;
using DVSRegister.Models.Home;
using DVSRegister.Models.Register;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("home")]
    public class HomeController(IHomeService homeService, ILogger<HomeController> logger) : BaseController(logger)
    {
        private readonly IHomeService homeService = homeService;

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
    }
}

[HttpGet("all-providers")]
public async Task<IActionResult> AllProviders(string currentSort = "date", string currentSortAction = "descending", string newSort = "", string searchText = "",
string searchAction = "", int pageNumber = 1)
{
    if (!string.IsNullOrEmpty(newSort))
    {
        if (currentSort == newSort)
        {
            currentSortAction = currentSortAction == "ascending" ? "descending" : "ascending";
        }
        else
        {
            currentSort = newSort;
            currentSortAction = "ascending";
        }
        pageNumber = 1;
    }

    if (searchAction == "clearSearch")
    {
        ModelState.Clear();
        searchText = string.Empty;
    }

    if (pageNumber < 1)
    {
        pageNumber = 1;
    }

    var results = await homeService.GetAllProviders(pageNumber, currentSort, currentSortAction, searchText);
    var totalPages = (int)Math.Ceiling((double)results.TotalCount / 10);

    var vm = new AllProvidersViewModel
    {
        Providers = results.Items,
        CurrentSort = currentSort,
        CurrentSortAction = currentSortAction,
        SearchText = searchText,
        TotalPages = totalPages
    };

    ViewBag.CurrentPage = pageNumber;

    return View(vm);
}


