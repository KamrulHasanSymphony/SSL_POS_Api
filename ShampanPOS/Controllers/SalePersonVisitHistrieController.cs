using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using System;
using System.Threading.Tasks;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalePersonVisitHistrieController : ControllerBase
    {
        SalePersonVisitHistrieService _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
        // POST: api/SalePersonVisitHistrie/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SalePersonVisitHistrieVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _salePersonVisitHistrieService = new SalePersonVisitHistrieService();

            try
            {
                resultVM = await _salePersonVisitHistrieService.Insert(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/SalePersonVisitHistrie/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SalePersonVisitHistrieVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
              _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                resultVM = await _salePersonVisitHistrieService.Update(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/SalePersonVisitHistrie/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
           _salePersonVisitHistrieService = new SalePersonVisitHistrieService();

                resultVM = await _salePersonVisitHistrieService.Delete(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/SalePersonVisitHistrie/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
              _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                resultVM = await _salePersonVisitHistrieService.List(new[] { "M.Id", "M.SalePersonId", "M.RouteId", "M.DATE" }, new[] {vm.Id,vm.SalePersonId, vm.RouteId, vm.ToDate }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/SalePersonVisitHistrie/CostomerList
        [HttpPost("CostomerList")]
        public async Task<ResultVM> CostomerList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                resultVM = await _salePersonVisitHistrieService.CostomerList(new[] { "M.Id" }, new[] { vm.Id }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // GET: api/SalePersonVisitHistrie/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SalePersonVisitHistrieVM uomConversion)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
               _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                resultVM = await _salePersonVisitHistrieService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // GET: api/SalePersonVisitHistrie/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                resultVM = await _salePersonVisitHistrieService.Dropdown(); // Adjust if Dropdown requires a different method
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/SalePersonVisitHistrie/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string salePersonId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                
                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                if (!string.IsNullOrEmpty(options.vm.BranchId))
                {
                    conditionFields.Add("H.BranchId");
                    conditionValues.Add(options.vm.BranchId.ToString());
                }

                if (!string.IsNullOrEmpty(options.vm.FromDate))
                {
                    conditionFields.Add("H.Date between");
                    conditionValues.Add(options.vm.FromDate.ToString());
                }

                if (!string.IsNullOrEmpty(options.vm.ToDate))
                {
                    conditionFields.Add("H.Date between");
                    conditionValues.Add(options.vm.ToDate.ToString());
                }

                if (!string.IsNullOrEmpty(salePersonId))
                {
                    conditionFields.Add("H.SalePersonId");
                    conditionValues.Add(salePersonId);
                    options.vm.SalePersonId = salePersonId;
                }

               

                string[] finalConditionFields = conditionFields.ToArray();
                string[] finalConditionValues = conditionValues.ToArray();
                _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                //resultVM = await _salePersonVisitHistrieService.GetGridData(options, salePersonId);
                resultVM = await _salePersonVisitHistrieService.GetGridData(options, salePersonId, finalConditionFields, finalConditionValues);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }



        [HttpPost("GetAllGridData")]
        public async Task<ResultVM> GetAllGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {

                List<string> conditionFields = new List<string>
                 {
                     "H.BranchId",
                     "H.Date between",
                     "H.Date between"
                 };

                List<string> conditionValues = new List<string>
                 {
                     options.vm.BranchId.ToString(),
                     options.vm.FromDate.ToString(),
                     options.vm.ToDate.ToString()
                 };

                //if (!string.IsNullOrEmpty(options.vm.UserId))
                //{
                //    conditionFields.Add("H.SalePersonId");
                //    conditionValues.Add(options.vm.UserId);

                //}

                string[] finalConditionFields = conditionFields.ToArray();
                string[] finalConditionValues = conditionValues.ToArray();

                _salePersonVisitHistrieService = new SalePersonVisitHistrieService();
                resultVM = await _salePersonVisitHistrieService.GetAllGridData(options, finalConditionFields, finalConditionValues);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }




    }
}
