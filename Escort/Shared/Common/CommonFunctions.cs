using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Shared.Common.Enums;
using Shared.Model.Request.AdminUser;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Resources;
using Shared.Model.Response;

namespace Shared.Common
{

    public class CommonFunctions
    {

        public static string GetDescription(Enum value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute =
                enumMember == null
                    ? default
                    : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return
                descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }

        public static List<string> SaveFile(List<IFormFile> files, string subDirectory, string imgPrefix = "")
        {
            List<string> tempFileAddress = new();
            subDirectory ??= string.Empty;
            var target = SiteKeys.SitePhysicalPath + "\\wwwroot" + subDirectory;

            Directory.CreateDirectory(target);

            files.ForEach(file =>
            {
                if (file.Length <= 0) return;

                var nFilename = string.Format(imgPrefix + "{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
                var filePath = Path.Combine(target, nFilename);
                tempFileAddress.Add(nFilename);
                using var stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);
            });
            return tempFileAddress;
        }


        public static string GetRelativeFilePath(string? fileName, string folderPath, string defaultImage)
        {
            return string.Format("{0}{1}{2}", SiteKeys.SiteUrl, folderPath, fileName ?? defaultImage);
        }

        public static string CentimetresToFeetInchesString(double centimetres, string footSymbol = "''", string inchesSymbol = "'")
        {
            (var feet, var inches) = CentimetresToFeetInches(centimetres);
            return $"{feet:N0}{footSymbol} {inches:N0}{inchesSymbol}";
        }

        public static (int Feet, double Inches) CentimetresToFeetInches(double centimetres)
        {
            var feet = centimetres / 2.54 / 12;
            var iFeet = (int)feet;
            var inches = (feet - iFeet) * 12;

            return (iFeet, inches);
        }

        public static FileContentResult GenerateClientBalanceExcelReport(GetClientBalanceReportDto data, string sheetName, string reportFileName, List<string>? dateColumns = null, List<string>? boolColumns = null)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);
                var currentRow = 1;

                // Add column headers
                int column = 1;

                worksheet.Cell(2, 1).Value = "Name";
                worksheet.Cell(2, 1).Style.Font.Bold = true;
                worksheet.Cell(2, 2).Value = data?.Client?.ClientName;

                worksheet.Cell(3, 1).Value = "Email";
                worksheet.Cell(3, 1).Style.Font.Bold = true;
                worksheet.Cell(3, 2).Value = data?.Client?.Email;

                worksheet.Cell(4, 1).Value = "Gender";
                worksheet.Cell(4, 1).Style.Font.Bold = true;
                worksheet.Cell(4, 2).Value = data?.Client?.Gender;

                worksheet.Cell(5, 1).Value = "Mobile";
                worksheet.Cell(5, 1).Style.Font.Bold = true;
                var phoneNumberWithCode = data?.Client?.CountryCode;
                if (!string.IsNullOrEmpty(phoneNumberWithCode) && !string.IsNullOrEmpty(data?.Client?.PhoneNumber))
                {
                    phoneNumberWithCode += "-" + data?.Client?.PhoneNumber;
                }
                else
                {
                    phoneNumberWithCode = data?.Client?.PhoneNumber;
                }

                worksheet.Cell(5, 2).Value = phoneNumberWithCode;

                worksheet.Cell(6, 1).Value = "Country";
                worksheet.Cell(6, 1).Style.Font.Bold = true;
                worksheet.Cell(6, 2).Value = data?.Client?.Country;



                worksheet.Cell("B9").Value = "Purchase Details";
                worksheet.Range("B9:D9").Merge().Style
                    .Font.Bold = true;
                worksheet.Range("B9:D9").Merge().Style.Font.FontSize = 14;
                worksheet.Range("B9:D9").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Add sub-headers
                worksheet.Cell("A10").Value = "Date"; // Assuming you have an Item column
                worksheet.Cell("B10").Value = "Transaction ID";
                worksheet.Cell("C10").Value = "Purchased Points";
                worksheet.Cell("D10").Value = "Amount";


                worksheet.Cell("E9").Value = "Spent Details";
                worksheet.Range("E9:G9").Merge().Style
                    .Font.Bold = true;
                worksheet.Range("E9:G9").Merge().Style.Font.FontSize = 14;
                worksheet.Range("E9:G9").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Add sub-headers
                worksheet.Cell("E10").Value = "Escort Name"; // Assuming you have an Item column
                worksheet.Cell("F10").Value = "Transaction Type";
                worksheet.Cell("G10").Value = "Point Spent";
                worksheet.Cell("H10").Value = "Point Balance";

                worksheet.Range("A10:H10").Style.Font.Bold = true;
                worksheet.Range("A9:H9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Range("A10:H10").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                // Group columns B, C, D under the Purchase Details header
                worksheet.Column("B").Group();
                worksheet.Column("C").Group();
                worksheet.Column("D").Group();

                // Set column widths for better readability
                worksheet.Column("A").Width = 20; // Width for Item
                worksheet.Column("B").Width = 20; // Width for Transaction ID
                worksheet.Column("C").Width = 15; // Width for Amount
                worksheet.Column("D").Width = 15; // Width for Token
                worksheet.Column("E").Width = 20;
                worksheet.Column("F").Width = 15;
                worksheet.Column("G").Width = 15;
                worksheet.Column("H").Width = 20;

                currentRow = 10;

                // Add data rows
                if (data != null && data.Transactions?.Count > 0)
                {
                    var columnMapping = new Dictionary<string, string>();
                    columnMapping.Add("Date", "Date");
                    columnMapping.Add("TransactionId", "Transaction Id");
                    columnMapping.Add("PointsPurchased", "Points Purchased");
                    columnMapping.Add("PointsPurchasedAmount", "Amount");
                    columnMapping.Add("UserDisplayName", "Escort Name");
                    columnMapping.Add("TransactionType", "Transaction Type");
                    columnMapping.Add("PointsSpent", "Points Spent");
                    columnMapping.Add("PointsBalance", "Points Balance");

                    foreach (var item in data.Transactions)
                    {
                        currentRow++;
                        column = 1;

                        foreach (var propertyName in columnMapping.Keys)
                        {
                            var property = typeof(GetClientTransactionReportDto).GetProperty(propertyName);
                            if (property != null)
                            {
                                var cell = worksheet.Cell(currentRow, column);
                                var value = property.GetValue(item, null);
                                if (dateColumns != null && dateColumns.Contains(propertyName) && value is DateTime dateValue)
                                {
                                    cell.Value = dateValue.ToShortDateString();
                                }
                                else if (value is not null) // Check if value is not null
                                {
                                    // Set the cell value based on its type
                                    if (value is short)
                                    {
                                        cell.Value = "Gift"; // Set string value directly
                                    }
                                    else if (value is decimal decimalValue)
                                    {
                                        cell.Value = decimalValue;
                                        cell.Style.NumberFormat.Format = "#,##0.00";
                                    }
                                    else
                                    {
                                        cell.Value = Convert.ToString(value);
                                    }
                                }
                                else
                                {
                                    cell.Value = string.Empty; // Set cell to empty string if value is null
                                }


                            }
                            column++;
                        }
                    }

                    currentRow = currentRow + 1;
                    worksheet.Range($"A{currentRow}:H{currentRow}").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    currentRow = currentRow + 1;
                    worksheet.Cell($"B{currentRow}").Value = "Total";

                    worksheet.Cell($"C{currentRow}").Value = data.Transactions.Sum(x => x.PointsPurchased);
                    worksheet.Cell($"C{currentRow}").Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell($"D{currentRow}").Value = data.Transactions.Sum(x => x.PointsPurchasedAmount);
                    worksheet.Cell($"D{currentRow}").Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell($"G{currentRow}").Value = data.Transactions.Sum(x => x.PointsSpent);
                    worksheet.Cell($"G{currentRow}").Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell($"H{currentRow}").Value = data.Transactions?.LastOrDefault()?.PointsBalance;
                    worksheet.Cell($"H{currentRow}").Style.NumberFormat.Format = "#,##0.00";

                    worksheet.Range($"A{currentRow}:H{currentRow}").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Range($"B{currentRow}:H{currentRow}").Style.Font.Bold = true;
                }

                // Save the workbook and return as a file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return new FileContentResult(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = reportFileName
                    };
                }
            }
        }



        public static FileContentResult GenerateExcelReport<T>(List<T> data, Dictionary<string, string> columnMapping, string sheetName, string reportFileName, List<string>? dateColumns = null, List<string>? boolColumns = null)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);
                var currentRow = 1;

                // Add column headers
                int column = 1;
                foreach (var columnName in columnMapping.Values)
                {
                    worksheet.Cell(currentRow, column).Value = columnName;
                    column++;
                }

                // Add data rows
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        currentRow++;
                        column = 1;

                        foreach (var propertyName in columnMapping.Keys)
                        {
                            var property = typeof(T).GetProperty(propertyName);
                            if (property != null)
                            {
                                var cell = worksheet.Cell(currentRow, column);
                                var value = property.GetValue(item, null);
                                if (dateColumns != null && dateColumns.Contains(propertyName) && value is DateTime dateValue)
                                {
                                    cell.Value = dateValue.ToString("MM/dd/yyyy");
                                    //cell.Style.DateFormat.Format = "MM/dd/yyyy"; // Apply date format
                                }
                                else if (boolColumns != null && boolColumns.Contains(propertyName) && value is bool booleanValue)
                                {
                                    cell.Value = booleanValue ? "Yes" : "No";
                                }
                                else
                                {
                                    cell.Value = value?.ToString() ?? string.Empty;
                                }


                                //worksheet.Cell(currentRow, column).Value = property.GetValue(item, null)?.ToString() ?? string.Empty;
                            }
                            column++;
                        }
                    }
                }

                // Save the workbook and return as a file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return new FileContentResult(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = reportFileName
                    };
                }
            }
        }


        public static string GetAlphanumericRandomPassword(int length)
        {
            const string alphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rng = new Random();
            var randomPassword = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                // Get a random character from the alphanumeric character set.
                var randomChar = alphanumericChars[rng.Next(alphanumericChars.Length)];
                randomPassword.Append(randomChar);
            }

            return randomPassword.ToString();
        }

        public static ActivityLog? GetActivityLogModel(ActivityLogRequestModel activityLogRequest)
        {
            var actionDescription = GetModifiedActivityDescription(activityLogRequest.LoggedInUser, activityLogRequest.TargetUser, activityLogRequest.ActivityType, activityLogRequest.Amount);

            if (string.IsNullOrEmpty(actionDescription))
            {
                return null;
            }

            ActivityLog activityLog = new ActivityLog()
            {
                TargetID = activityLogRequest.TargetId,
                DbEntityType = activityLogRequest.DbEntity,
                AdminUserID = activityLogRequest.LoggedInUser.UserId,
                ActionDate = DateTime.UtcNow,
                ActionType = activityLogRequest.ActivityType.ToString(),
                ActionDescription = actionDescription,
            };

            return activityLog;
        }

        private static string GetModifiedActivityDescription(ActivityUser loggedInUser, ActivityUser? targetUser, ActivityType activityType, decimal amount)
        {
            string actionDescription = GetActivityDescription(activityType);

            actionDescription = actionDescription.Replace("##AdminUserName##", loggedInUser.UserName)
                                     .Replace("##AdminUserRole##", loggedInUser.UserRole)
                                     .Replace("##TargetUserName##", targetUser?.UserName ?? string.Empty)
                                     .Replace("##TargetUserRole##", targetUser?.UserRole ?? string.Empty);

            if (ActivityType.MarkPayment == activityType)
            {
                actionDescription = actionDescription.Replace("##Amount##", amount.ToString());
            }
            
            return actionDescription;
        }

        private static string GetActivityDescription(ActivityType activityType)
        {
            return activityType switch
            {
                ActivityType.CreateProfile => ResourceString.AdminUserCreateProfile,
                ActivityType.UpdateProfile => ResourceString.AdminUserUpdateProfile,
                ActivityType.DeActivateProfile => ResourceString.AdminUserDeActivateProfile,
                ActivityType.ActivateProfile => ResourceString.AdminUserActivateProfile,
                ActivityType.MarkPayment => ResourceString.AdminUserMarkPaymentDone,
                ActivityType.ExportClientBalanceReport => ResourceString.AdminUserExportClientBalanceReport,
                ActivityType.ExportPaymentReport => ResourceString.AdminUserExportPaymentReport,
                ActivityType.ApproveProfile => ResourceString.AdminUserApproveProfile,
                ActivityType.DeniedProfile => ResourceString.AdminUserDeniedProfile,
                _ => string.Empty // Default case if no match found
            };
        }

        public static string GetLocationFromIp(string ipAddress)
        {

            if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "Unknown IP")
                return ("Unknown Location");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = $"http://ip-api.com/json/{ipAddress}";
                    var response = httpClient.GetAsync(apiUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = response.Content.ReadAsStringAsync().Result;
                        var locationData = System.Text.Json.JsonDocument.Parse(jsonResponse);

                        string state = locationData.RootElement.GetProperty("regionName").GetString() ?? "Unknown State";
                        string city = locationData.RootElement.GetProperty("city").GetString() ?? "Unknown City";

                        if (string.IsNullOrEmpty(state))
                        {
                            return $"{city}";

                        }
                        return $"{state}, {city}";

                    }
                    else
                    {
                        return ("Unknown Location");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving location: {ex.Message}");
                return ("Unknown Location");
            }
        }
    }
}
