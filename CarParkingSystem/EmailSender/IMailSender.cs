namespace CarParkingSystem.EmailSender
{
    public interface IMailSender
    {
        public void MessageSend(Message message);
    }
}
