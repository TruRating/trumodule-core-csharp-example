using System;
using TruRating.Dto.TruService.V220;

namespace TruRating.TruModule
{
    class TruModuleHelpers
    {
        internal static bool QuestionAvailable(Response response, string language, out ResponseQuestion responseQuestion,
            out ResponseReceipt[] responseReceipts, out ResponseScreen[] responseScreens)
        {
            responseQuestion = null;
            responseReceipts = null;
            responseScreens = null;
            if (response == null) //TODO check for null?
            {
                throw new ArgumentNullException(nameof(response));
            }
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }
            var item = response.Item as ResponseDisplay;
            if (item != null)
            {
                var responseDisplay = item;
                if (responseDisplay.Language != null)
                    foreach (var responseLanguage in responseDisplay.Language)
                    {
                        if (language.Equals(responseLanguage.Rfc1766) && responseLanguage.Question != null)
                        {
                            responseQuestion = responseLanguage.Question;
                            responseReceipts = responseLanguage.Receipt;
                            responseScreens = responseLanguage.Screen;
                            return true;
                        }
                    }
                return false;
            }

            return false;
        }

        internal static string GetResponseReceipt(ResponseReceipt[] responseReceipts, When when)
        {
            if (responseReceipts == null)
                return null;
            foreach (var responseReceipt in responseReceipts)
            {
                if (responseReceipt.When == when)
                {
                    return responseReceipt.Value;
                }
            }
            return null;
        }

        internal static ResponseScreen GetResponseScreen(ResponseScreen[] responseScreens, When whenToDisplay)
        {
            if (responseScreens == null)
                return null;
            foreach (var responseScreen in responseScreens)
            {
                if (responseScreen.When == whenToDisplay) //If this response element matches the state of the screen.
                {
                    return responseScreen;
                }
            }
            return null;
        }
    }
}