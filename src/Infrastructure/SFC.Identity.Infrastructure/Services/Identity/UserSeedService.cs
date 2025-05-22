using AutoMapper;

using MassTransit;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Application.Interfaces.Metadata;
using SFC.Identity.Application.Interfaces.Persistence.Repository.User;
using SFC.Identity.Domain.Entities.User;
using SFC.Identity.Infrastructure.Persistence.Entities;
using SFC.Identity.Infrastructure.Persistence.Repositories.User;
using SFC.Identity.Messages.Events.User;

namespace SFC.Identity.Infrastructure.Services.Identity;
public class UserSeedService(
    IMapper mapper,
    IPublishEndpoint publisher,
    IUserRepository userRepository,
    IMetadataService metadataService) : IUserSeedService
{
    private readonly IMapper _mapper = mapper;
    private readonly IPublishEndpoint _publisher = publisher;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMetadataService _metadataService = metadataService;

    #region Stub data

    private const string TEST_PASSWORD = "Test1234!";

    private static readonly List<Guid> USER_IDS =
    [
        Guid.Parse("{836A7FB9-73FE-4107-91F2-4D916158E242}"),
        Guid.Parse("{615C2665-1D2A-4C36-A675-3AD6E466670C}"),
        Guid.Parse("{D635FA2A-670A-4C10-A271-751BE1A353FE}"),
        Guid.Parse("{33D2A91B-394C-4B8E-9EE2-A417D808BCA2}"),
        Guid.Parse("{7E9B486C-1CC1-4957-B1F1-D265254DC21A}"),
        Guid.Parse("{533D01E4-216C-43DF-AF5B-DE972B2A0352}"),
        Guid.Parse("{4FF1F686-F83B-407A-9677-DE62DCDD4A7F}"),
        Guid.Parse("{EBDDA7F0-E6B7-485C-854B-CF0013CF6D7C}"),
        Guid.Parse("{F4025067-6352-45C0-AD08-61C64334FB49}"),
        Guid.Parse("{A3B507F9-0AE9-45FF-8F43-BAE1471B84D3}"),
        Guid.Parse("{2E55FD9C-4B42-4A7E-A24E-A9F7049C4386}"),
        Guid.Parse("{49E9084B-6289-4535-90A1-0027CF37999C}"),
        Guid.Parse("{39DF6D95-1D9C-480F-98EF-9C3EB46D4021}"),
        Guid.Parse("{56AD4610-9D6A-480F-AADF-212CCE22EEB9}"),
        Guid.Parse("{BAD1711F-D928-4CE7-9F4C-210F736A8DAB}"),
        Guid.Parse("{CA7DEC50-1A43-4FE1-BC05-5715C6E8DC12}"),
        Guid.Parse("{676C9528-248F-4368-9682-A2FBE0672FD5}"),
        Guid.Parse("{4FA8AC27-574A-4C3B-A125-30E39DD35970}"),
        Guid.Parse("{9F2F6C26-A315-4F9A-ABF4-8A6904685B85}"),
        Guid.Parse("{08CA89FD-8751-41AE-B493-E4C87006EB6D}"),
        Guid.Parse("{74C68915-C449-4BD4-8608-B1469AC7AC6F}"),
        Guid.Parse("{966FC1A2-9FDC-4BED-98EA-0562C900DC6F}"),
        Guid.Parse("{16A37EC9-D7C7-49EF-87FB-554D3AF1935F}"),
        Guid.Parse("{1BFD94C8-CCF9-46A6-B273-D2AD67920D20}"),
        Guid.Parse("{628CE64E-9AA9-4195-A4C0-37949B0A661B}"),
        Guid.Parse("{A4638E2B-7F63-41E1-98C2-99F646F56507}"),
        Guid.Parse("{2A2C1807-D318-403E-A3D1-053C17BACD7D}"),
        Guid.Parse("{54D339C8-7087-4163-9E80-CF9EC6D668AB}"),
        Guid.Parse("{88497260-3E25-4691-B5DE-0C8A4A66AE43}"),
        Guid.Parse("{D278FC81-91B9-49B0-8F14-A5DC67055119}"),
        Guid.Parse("{57C06548-F80E-4FD0-BA7A-5D429FEA19B0}"),
        Guid.Parse("{4BC216BF-016B-496D-BC5A-CA4A18F307AF}"),
        Guid.Parse("{F12E2358-4B77-49EB-BF8B-7B10FC0EA814}"),
        Guid.Parse("{4288D160-B7A0-4278-BD0F-4008D44260F9}"),
        Guid.Parse("{A7857160-EBEA-4442-BD78-80B4A5F007E7}"),
        Guid.Parse("{21A1F309-7A4B-452A-9410-4513AB694A6C}"),
        Guid.Parse("{C4A98031-9679-4908-9207-D248D59E3043}"),
        Guid.Parse("{DDA85392-2B39-4ADC-9924-6D79856DEA08}"),
        Guid.Parse("{CF1C5EE4-9789-4057-BA9E-6000A7FF2FF1}"),
        Guid.Parse("{DDD0BF23-08F6-4D5C-B7EA-76BB2BCFE7AB}"),
        Guid.Parse("{B651D703-C8C8-4615-B16A-B067E760BA61}"),
        Guid.Parse("{9F9518E0-CD78-43C0-ACC7-99535824726F}"),
        Guid.Parse("{9CED5A7C-EFD1-4E1A-8E35-F1CC913019DC}"),
        Guid.Parse("{9B76F79D-AE26-4732-8194-7F73E6CB6FED}"),
        Guid.Parse("{B8F05267-A24D-4107-90DC-1CCBED72D83E}"),
        Guid.Parse("{A998CDB9-7DE0-454A-9D67-55112D4EE3CE}"),
        Guid.Parse("{9E58C03B-0006-4FF7-AF7A-B31A9CF572B1}"),
        Guid.Parse("{EBF55C19-7BEA-48C0-931A-5866B29EAC12}"),
        Guid.Parse("{23E19EAD-291B-4CC9-BA59-8A2A64A6FE00}"),
        Guid.Parse("{882A446B-B09C-4432-A69A-67365FAA00C4}"),
        Guid.Parse("{40865404-0AE8-479A-95DE-67E10B6539B2}"),
        Guid.Parse("{7EB4A491-B8C6-4613-A884-36A8520DA354}"),
        Guid.Parse("{86A6A19F-D93A-4366-8C18-E4149FCCD112}"),
        Guid.Parse("{004A072C-3192-411D-AA6F-0067B69F2096}"),
        Guid.Parse("{F3B4362F-CD5A-4FC9-B9C7-21816DC34A40}"),
        Guid.Parse("{5FD0103F-53CB-48CE-AE79-7E44C4A68EC3}"),
        Guid.Parse("{FDF00E00-B8D8-41CA-98CC-66EB59068CD8}"),
        Guid.Parse("{F6DA9D2A-312C-404B-9E17-BF435C24337B}"),
        Guid.Parse("{9D794C56-B380-4B1B-B295-C0B619EF933A}"),
        Guid.Parse("{D329F3DA-2E30-4F18-A1E9-BDFC9AEF993A}"),
        Guid.Parse("{45FAB075-7904-45DF-A816-239D6DE488DC}"),
        Guid.Parse("{8D8F2328-0872-402F-8263-5C1EB491E0B9}"),
        Guid.Parse("{7CC35CE6-8F4F-4817-903C-90B9B7A54DAA}"),
        Guid.Parse("{10E6E79A-0C32-4E7A-8115-21B2549E8EF5}"),
        Guid.Parse("{B7F8D7C4-D2E3-402E-BDBA-C40D46AACE9A}"),
        Guid.Parse("{2D149FE6-898D-464F-A547-F5775D6B3975}"),
        Guid.Parse("{A1B3F5C5-D912-4A2A-8BEB-9BE16F91B5A7}"),
        Guid.Parse("{ED793F8F-5922-4F5D-B09B-1F52CF876FF0}"),
        Guid.Parse("{1E60A0ED-4867-4AAE-8E37-86F32131BDF6}"),
        Guid.Parse("{A490B588-C7BC-4C8B-83D0-AEC774883090}"),
        Guid.Parse("{FF04AB79-91AD-4C8F-87FF-BCDF34414E79}"),
        Guid.Parse("{3713E4B7-BCDE-4D06-864B-68FC6FC9A528}"),
        Guid.Parse("{658AC968-3430-48C8-A385-8B727373E464}"),
        Guid.Parse("{040076D8-96AB-4493-99A8-B16BF44E6269}"),
        Guid.Parse("{A7B4D868-4679-4199-BDD0-086DC08DABDF}"),
        Guid.Parse("{EA968F41-303D-4287-8FB0-5C7500D3DEC5}")
    ];

    private static readonly Guid USER_WITHOUT_PROFILE_ID = Guid.Parse("{B0788D2F-8003-43C1-92A4-EDC76A7C5DDE}");

    #endregion Stub data

    #region Public

    public async Task<IEnumerable<IUser>> GetSeedUsersAsync()
    {
        return await _userRepository.GetByIdsAsync([.. USER_IDS, .. new[] { USER_WITHOUT_PROFILE_ID }]).ConfigureAwait(true);
    }

    public async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        ApplicationUser withoutProfileUser = BuildUserWithoutProfile();

        IEnumerable<ApplicationUser> profileUsers = USER_IDS.Select(BuildUser);

        IEnumerable<ApplicationUser> users = [.. profileUsers, .. new[] { withoutProfileUser }];

        IUser[] seedUsers = await _userRepository.AddRangeAsync([.. users]).ConfigureAwait(true);

        await PublishUsersSeededEvent(seedUsers, cancellationToken).ConfigureAwait(true);

        await _metadataService.CompleteAsync(MetadataServiceEnum.Identity, MetadataDomainEnum.User, MetadataTypeEnum.Seed).ConfigureAwait(true);
    }

    #endregion Public

    #region Private

    private static ApplicationUser BuildUser(Guid userId, int index)
    {
        DateTime createdDate = DateTime.UtcNow;

        return new()
        {
            Id = userId,
            CreatedBy = userId,
            CreatedDate = createdDate,
            LastModifiedBy = userId,
            LastModifiedDate = createdDate,
            UserName = $"TestProfileUser{index}",
            NormalizedUserName = $"TESTPROFILEUSER{index}",
            Email = $"testprofile{index}email@mail.com",
            NormalizedEmail = $"TESTPROFILE{index}EMAIL@MAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, TEST_PASSWORD)
        };
    }

    private static ApplicationUser BuildUserWithoutProfile()
    {
        Guid userId = USER_WITHOUT_PROFILE_ID;
        DateTime createdDate = DateTime.UtcNow;

        return new()
        {
            Id = userId,
            CreatedBy = userId,
            CreatedDate = createdDate,
            LastModifiedBy = userId,
            LastModifiedDate = createdDate,
            UserName = "TestUser",
            NormalizedUserName = "TESTUSER",
            Email = "testemail@mail.com",
            NormalizedEmail = "TESTEMAIL@MAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, TEST_PASSWORD)
        };
    }

    private Task PublishUsersSeededEvent(IEnumerable<IUser> users, CancellationToken cancellationToken)
    {
        UsersSeeded @event = _mapper.Map<UsersSeeded>(users);
        return _publisher.Publish(@event, cancellationToken);
    }

    #endregion Private
}