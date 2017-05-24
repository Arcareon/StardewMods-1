﻿namespace Entoarox.Framework
{
    interface IConditionHelper
    {
        bool ValidateConditions(string conditions, char separator = ',');
        bool ValidateConditions(string[] conditions);
        bool ValidateCondition(string condition);
    }
}
