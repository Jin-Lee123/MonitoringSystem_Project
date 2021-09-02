using System;
using System.Windows.Input;

namespace MonitoringSystem
{
    public class RelayCommand<T> : ICommand
    {

        #region ###대리자 선언###
        // View에서 넘어온 Command를 실행하는 대리자
        private readonly Action<T> execute;
        // View에서 넘어온 Command를 실행할 수 있는 지 확인하는 대리자
        private readonly Predicate<T> canExecute;
        #endregion

        #region ###ICommand 필수 내용###
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        // 명령을 실행할 수 있는지 확인
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke((T)parameter) ?? true;
        }
        // 명령을 실행
        public void Execute(object parameter)
        {
            execute.Invoke((T)parameter);
        }
        #endregion

        #region ###생성자 생성###
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            //RelayCommand 실행명령은 null일 수 없다.
            this.execute = execute ?? throw new ArgumentException(nameof(execute));
            this.canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
            this.execute = execute;
        }

        #endregion
    }
}
