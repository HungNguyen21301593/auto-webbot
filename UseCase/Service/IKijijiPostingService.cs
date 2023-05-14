using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Core.Model;

namespace UseCase.Service
{
    public interface IKijijiPostingService
    {
        void Execute(KijijiExecuteType kijijiExecuteType);
    }
}
