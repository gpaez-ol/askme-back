using Microsoft.Extensions.Options;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using AskMe.Data.Config;
using AskMe.Data.Context;
using AskMe.Repositories;

namespace AskMe.Repositories.Manager
{
    public class RepositoryManager
    {
        private UserRepository _userRepository;
        private PostRepository _postRepository;
        private readonly IOptions<AppConfig> _cloudConfig;
        private AskMeContext _context;
        private readonly IMapper _mapper;
        IConfiguration _configuration;
        public RepositoryManager(IOptions<AppConfig> config, AskMeContext context, IMapper mapper, IConfiguration configuration)
        {
            _cloudConfig = config;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public UserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_cloudConfig, _context, _mapper, _configuration);
                }
                return _userRepository;
            }
        }
        public PostRepository PostRepository
        {
            get
            {
                if (_postRepository == null)
                {
                    _postRepository = new PostRepository(_context);
                }
                return _postRepository;
            }
        }

        public void Dispose()
        {
            _context.SaveChanges();
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
