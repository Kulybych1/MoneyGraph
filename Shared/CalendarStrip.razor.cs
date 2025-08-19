using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalAccountant.Shared
{
    public partial class CalendarStrip
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public DateTime SelectedDate { get; set; }

        [Parameter]
        public EventCallback<DateTime> SelectedDateChanged { get; set; }

        private List<DateTime> _dates = new();
        private ElementReference _containerRef;
        private string _containerId = "calendar-strip";
        private DateTime _currentCenterDate = DateTime.Today;

        protected override void OnInitialized()
        {
            GenerateDates(_currentCenterDate);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ScrollToSelectedDate();
            }
        }

        private async Task SelectDate(DateTime date)
        {
            await SelectedDateChanged.InvokeAsync(date);
            await ScrollToSelectedDate(date);
        }

        private async Task ScrollToSelectedDate(DateTime? date = null)
        {
            var dateToScroll = date ?? SelectedDate;
            await JSRuntime.InvokeVoidAsync("scrollToDate", _containerId, $"date-cell-{dateToScroll:yyyy-MM-dd}");
        }

        // This logic ensures the 3-week view re-centers if needed, though not required by current spec
        private void GenerateDates(DateTime centerDate)
        {
            _dates.Clear();
            var startDate = centerDate.AddDays(-10);
            for (int i = 0; i < 21; i++)
            {
                _dates.Add(startDate.AddDays(i));
            }
        }

        private string GetDateCellCss(DateTime date)
        {
            if (date.Date == SelectedDate.Date) return "date-cell selected";
            if (date.Date == DateTime.Today) return "date-cell today";
            return "date-cell";
        }
    }
}