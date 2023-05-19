﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Infrastructure.Domain
{
    /// <summary>
    /// Stores a rule and it's related property
    /// </summary>
    public class BusinessRule
    {
        private string _property;
        private string _rule;

        public BusinessRule(string property, string rule)
        {
            this._property = property;
            this._rule = rule;
        }

        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public string Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }
    }

}
